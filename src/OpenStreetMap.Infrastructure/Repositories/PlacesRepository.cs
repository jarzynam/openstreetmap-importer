using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using OpenStreetMap.Common;
using OpenStreetMap.Infrastructure.Databases;
using OpenStreetMap.Infrastructure.Repositories.Entities;

namespace OpenStreetMap.Infrastructure.Repositories
{
    public interface IPlacesRepository
    {
        Task AddOrUpdateAsync(List<PlaceEntity> places);
        Task<List<PlaceEntity>> GetAsync(PlaceType type);
        Task<List<PlaceEntity>> GetNearAsync(CoordinatesModel coordinates, double? maxDistance);

        Task<List<PlaceEntity>> GetAllAsync(List<PlaceType> placeTypes);
    }

    public class PlacesRepository : IPlacesRepository
    {
        private readonly IPlacesDatabase _database;

        public PlacesRepository(IPlacesDatabase database)
        {
            _database = database;
        }

        public async Task AddOrUpdateAsync(List<PlaceEntity> places)
        {
            var replaceRequests = new List<ReplaceOneModel<PlaceEntity>>();

            foreach (var placeItem in places)
            {
                replaceRequests.Add(new ReplaceOneModel<PlaceEntity>(CreateCoordinatesFilter(placeItem.OsmId), placeItem)
                {
                    IsUpsert = true
                });
            }
            
            if (replaceRequests.Count > 0)
                await BulkWriteWithRetry(replaceRequests);
        }


        public async Task<List<PlaceEntity>> GetAsync(PlaceType placeType)
        {
            var results = new List<PlaceEntity>();

            var filter = CreatePlaceTypeFilter(placeType);

            var document = await _database.OsmPlaces.Find(filter).FirstOrDefaultAsync();

            results.Add(document);

            return results;
        }

        public async Task<List<PlaceEntity>> GetNearAsync(CoordinatesModel coordinates, double? maxDistance)
        {
            var location = GeoJson.Point(GeoJson.Geographic(coordinates.Longitude, coordinates.Latitude));
            var filter = CreateNearFilter(maxDistance, location);

            return await _database.OsmPlaces.Find(filter).ToListAsync();
        }

        public async Task<List<PlaceEntity>> GetAllAsync(List<PlaceType> placeTypes)
        {
            var filter = Builders<PlaceEntity>.Filter.In(x => x.PlaceType, placeTypes);

            return await _database.OsmPlaces.Find(filter).ToListAsync();
        }

        private async Task BulkWriteWithRetry(IEnumerable<WriteModel<PlaceEntity>> requests)
        {
            var requestList = requests.ToList();

            try
            {
                await _database.OsmPlaces.BulkWriteAsync(requestList);
            }
            catch (MongoBulkWriteException exception)
            {
                int index = exception.WriteErrors[0].Index;

                await BulkWriteWithRetry(requestList.GetRange(index, requestList.Count - index));
            }
        }

        private static FilterDefinition<PlaceEntity> CreatePlaceTypeFilter(PlaceType placeType)
        {
            return Builders<PlaceEntity>.Filter.Eq(x => x.PlaceType, placeType);
        }

        private static FilterDefinition<PlaceEntity> CreateCoordinatesFilter(long osmId)
        {
            return Builders<PlaceEntity>.Filter.Eq(x => x.OsmId, osmId);
                
        }

        private static FilterDefinition<PlaceEntity> CreateNearFilter(double? maxDistance, GeoJsonPoint<GeoJson2DGeographicCoordinates> location)
        {
            return Builders<PlaceEntity>.Filter.Near(x => x.Location, location, maxDistance);
        }
    }
}