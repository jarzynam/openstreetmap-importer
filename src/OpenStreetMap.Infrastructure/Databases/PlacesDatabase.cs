using MongoDB.Driver;
using OpenStreetMap.Infrastructure.Repositories.Entities;

namespace OpenStreetMap.Infrastructure.Databases
{
    public interface IPlacesDatabase
    {
        IMongoCollection<PlaceEntity> OsmPlaces { get; }
        Task UpdateIndexesAsync();
    }

    public class PlacesDatabase : IPlacesDatabase
    {
        private readonly IMongoDatabase _database;
        private const string Version = "2";
        private const string DatabaseName = "places";

        public PlacesDatabase(MongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase(DatabaseName);
        }

        public IMongoCollection<PlaceEntity> OsmPlaces =>
            _database.GetCollection<PlaceEntity>($"osmV{Version}");

        public async Task UpdateIndexesAsync()
        {
            var builder = new IndexKeysDefinitionBuilder<PlaceEntity>();

            var geoIndex = builder.Geo2D(x => x.Location);
            var idIndex = builder.Hashed(x => x.OsmId);
           
            await OsmPlaces.Indexes.CreateManyAsync(
                new List<CreateIndexModel<PlaceEntity>> { new(geoIndex), new(idIndex) }
            );
        }
    }
}
