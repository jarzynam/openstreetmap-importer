using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using OpenStreetMap.Infrastructure.Databases;
using OpenStreetMap.Infrastructure.Repositories;

namespace OpenStreetMap.Infrastructure
{
    public static class InfrastructureModule
    {
        public static async Task<IPlacesRepository> CreatePlacesRepository(string mongoConnectionString)
        {
            var mongoClient = CreateMongoClient(mongoConnectionString);

            var db = new PlacesDatabase(mongoClient);
            await db.UpdateIndexesAsync();
            
            return new PlacesRepository(db);
        }

        private static MongoClient CreateMongoClient(string connectionString)
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, _ => true);

            return new MongoClient(connectionString);
        }
    }
}
