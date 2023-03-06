using MongoDB.Driver.GeoJsonObjectModel;
using OpenStreetMap.Common;

namespace OpenStreetMap.Infrastructure.Repositories.Entities
{
    public record PlaceEntity
    {
        public PlaceType PlaceType { get; init; }
        public GeoJson2DGeographicCoordinates? Location { get; init; }
        public string? OsmTagKey { get; init; }
        public long OsmId { get; init; }
        public string? OsmTagValue { get; init; }
        public string? Name { get; init; }
    }
}