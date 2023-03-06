using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenStreetMap.Common;
using OpenStreetMap.Importer.Importer.Helpers;
using OpenStreetMap.Infrastructure.Repositories;
using OpenStreetMap.Infrastructure.Repositories.Entities;
using OsmSharp;
using OsmSharp.Streams;
using OsmSharp.Tags;

namespace OpenStreetMap.Importer.Importer
{
    public class OpenStreetMapImporter
    {
        private readonly IPlacesRepository _placesRepository;
        private readonly Dictionary<long, CoordinatesModel> _coordinatesCache;
        private readonly OpenStreetMapMapper _openStreetMapMapper;
        private readonly Tag[] _tags;

        private const int BufferSize = 1000;
        private readonly TimeSpan _sleepTimeInMs = TimeSpan.FromSeconds(0);

        public OpenStreetMapImporter(IPlacesRepository placesRepository)
        {
            _placesRepository = placesRepository;
            _coordinatesCache = new Dictionary<long, CoordinatesModel>(100000000);

            
            _tags = PlaceGroups.Items.Select(x => new Tag(x.OpenStreetMapTagKey, x.OpenStreetMapTagValue)).ToArray();

            _openStreetMapMapper = new OpenStreetMapMapper(_coordinatesCache, _tags);
        }

        public async Task ImportAsync(string osmDatabaseFilePath)
        {
            Console.WriteLine("Start processing " + osmDatabaseFilePath);

            int nodeCounter = 0;
            int wayCounter = 0;

            await using var stream = new FileInfo(osmDatabaseFilePath).OpenRead();
            using var source = new PBFOsmStreamSource(stream);
            int nodeIndex = 0;
            int wayIndex = 0;

            var nodesBuffer = new Node[BufferSize];
            var waysBuffer = new Way[BufferSize];
            var resultBuffer = new PlaceEntity[BufferSize];

            while (source.MoveNext())
            {
                var current = source.Current();

                if (current == null || current.Type == OsmGeoType.Relation || current.Visible == false)
                    continue;

                if (current.Type == OsmGeoType.Node)
                {
                    var node = (Node)current;

                    if (HasSupportedTags(current))
                    {
                        nodesBuffer[nodeIndex++] = node;
                    }
                    else
                    {
                        _coordinatesCache.Add(node.Id.GetValueOrDefault(), new CoordinatesModel { Latitude = node.Latitude.GetValueOrDefault(), Longitude = node.Longitude.GetValueOrDefault() });

                        continue;
                    }
                }
                else if (HasSupportedTags(current))
                {
                    waysBuffer[wayIndex++] = (Way)current;
                }

                if (nodeIndex == BufferSize || wayIndex == BufferSize)
                {
                    if (nodeIndex > 0)
                    {
                        nodeCounter = await ProcessNodesAsync(nodesBuffer, resultBuffer, nodeIndex, nodeCounter);
                        Thread.Sleep(_sleepTimeInMs);
                    }

                    if (wayIndex > 0)
                    {
                        wayCounter = await ProcessWaysAsync(waysBuffer, resultBuffer, wayIndex, wayCounter);
                        Thread.Sleep(_sleepTimeInMs);
                    }

                    nodeIndex = 0;
                    wayIndex = 0;
                }
            }

            await ProcessNodesAsync(nodesBuffer, resultBuffer, nodeIndex, nodeCounter);
            await ProcessWaysAsync(waysBuffer, resultBuffer, wayIndex, wayCounter);
        }

        private async Task<int> ProcessWaysAsync(Way[] inputBuffer, PlaceEntity[] resultBuffer,
            int index, int counter)
        {
            _openStreetMapMapper.Map(inputBuffer, ref resultBuffer, index);

            var result = resultBuffer
                .Take(index)
                .ToList();

            await _placesRepository.AddOrUpdateAsync(result);

            counter += index;

            Console.WriteLine($"Processed {counter} ways");

            return counter;
        }

        private async Task<int> ProcessNodesAsync(Node[] inputBuffer, PlaceEntity[] resultBuffer,
            int index, int counter)
        {
            _openStreetMapMapper.Map(inputBuffer, ref resultBuffer, index);

            var result = resultBuffer
                .Take(index)
                .ToList();

            await _placesRepository.AddOrUpdateAsync(result);

            counter += index;
            Console.WriteLine($"Processed {counter} nodes");

            return counter;
        }

        private bool HasSupportedTags(OsmGeo node)
        {
            return _tags.Any(node.Tags.Contains);
        }
    }
}