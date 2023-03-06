using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.GeoJsonObjectModel;
using OpenStreetMap.Common;
using OpenStreetMap.Infrastructure.Repositories.Entities;
using OsmSharp;
using OsmSharp.Tags;

namespace OpenStreetMap.Importer.Importer.Helpers
{
    public class OpenStreetMapMapper
    {
        private readonly Tag[] _tags;
        private readonly Dictionary<string, PlaceType> _placeTypeByOsmCategory;
        private readonly Dictionary<long, CoordinatesModel> _coordinatesByNodeId;
        public OpenStreetMapMapper(Dictionary<long, CoordinatesModel> coordinatesByNodeId, Tag[] tags)
        {
            _coordinatesByNodeId = coordinatesByNodeId;

            _tags = tags;

            _placeTypeByOsmCategory = PlaceGroups.Items
                .Select(x => new { tag = x.OpenStreetMapTagValue.ToLowerInvariant(), type = x.PlaceType })
                .Distinct()
                .ToDictionary(x => x.tag, x => x.type);
        }

        public void Map(Way[] ways, ref PlaceEntity[] resultBuffer, int lastElementIndex)
        {
            for (var i = 0; i < lastElementIndex; i++)
            {
                var coordinates = new List<CoordinatesModel>(ways[i].Nodes.Length);

                // all nodes in a way were pre-iterated and put in the _coordinatesByNodeId by importer, we can skip others to save time
                for (int j = 0; j < ways[i].Nodes.Length; j++)
                {
                    if (!_coordinatesByNodeId.ContainsKey(ways[i].Nodes[j]))
                        continue;

                    coordinates.Add(_coordinatesByNodeId[ways[i].Nodes[j]]);
                }

                if (coordinates.Count == 0)
                    continue;

                var centerPoint = CentralPointCalculator.Calculate(coordinates);

                resultBuffer[i] = CreatePlaceEntity(ways[i], centerPoint);
            }
        }
        public void Map(Node[] nodes, ref PlaceEntity[] resultBuffer, int lastElementIndex)
        {
            for (var i = 0; i < lastElementIndex; i++)
            {
                var coordinates = new CoordinatesModel { Latitude = nodes[i].Latitude.GetValueOrDefault(), Longitude = nodes[i].Longitude.GetValueOrDefault() };

                resultBuffer[i] = CreatePlaceEntity(nodes[i], coordinates);
            }
        }

        private PlaceEntity CreatePlaceEntity(OsmGeo point, CoordinatesModel coordinates)
        {

            Tag? leadTag = null;

            for (var tagIndex = 0; tagIndex < _tags.Length; tagIndex++)
            {
                if (!point.Tags.Contains(_tags[tagIndex])) continue;

                leadTag = _tags[tagIndex];
                break;
            }

            if (!leadTag.HasValue)
                return null;

            var name = GetName(point);
            var placeType = _placeTypeByOsmCategory[leadTag.Value.Value.ToLowerInvariant()];

            return new PlaceEntity
            {
                PlaceType = placeType,
                Name = name,
                OsmId = point.Id.GetValueOrDefault(),
                OsmTagKey = leadTag.Value.Key,
                OsmTagValue = leadTag.Value.Value,
                Location = new GeoJson2DGeographicCoordinates(coordinates.Longitude, coordinates.Latitude)
            };
        }

        private static string GetName(OsmGeo point)
        {
            point.Tags.TryGetValue("name", out var name);

            return name;
        }
    }
}