using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using OpenStreetMap.Common;
using OpenStreetMap.Importer.Importer.Helpers;
using OpenStreetMap.Infrastructure.Repositories.Entities;
using OsmSharp;
using OsmSharp.Tags;

namespace OpenStreetMap.Tests.Unit.Importer.Helpers
{
    [TestClass]
    public class OpenStreetMapMapperTests
    {
        private Tag[] _tags;
        private IFixture _fixture;
        private Random _random;

        [TestInitialize]
        public void Initialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _tags = PlaceGroups.Items.Select(x => new Tag(x.OpenStreetMapTagKey, x.OpenStreetMapTagValue)).ToArray();
            _random = new Random(Guid.NewGuid().GetHashCode());

        }
        [TestMethod]
        public void MapWays_ShouldMapToPlaces_WhenHaveElements()
        {
            // arrange 
            int placesSize = 1000;
            int waysSize = 5;
            int nodesInAWay = 5;
           
            var coordinatesCache = new Dictionary<long, CoordinatesModel>();
            var ways = CreateWays(waysSize, nodesInAWay, coordinatesCache);

            var places = new PlaceEntity[placesSize];
            var mapper = new OpenStreetMapMapper(coordinatesCache, _tags);

            // act
            mapper.Map(ways, ref places, ways.Length);

            // assert
            places.Take(waysSize-1).Should().NotContainNulls();
            for (int i = 0; i < waysSize; i++)
            {
                places[i].Name.Should().Be(ways[i].Tags["name"]);
                places[i].OsmId.Should().Be(ways[i].Id);
                var coordinates = coordinatesCache[ways[i].Nodes.First()];
                places[i].Location?.Latitude.Should().Be(coordinates.Latitude);
                places[i].Location?.Longitude.Should().Be(coordinates.Longitude);
            }
        }

        private Way[] CreateWays(int waysSize, int nodesInAWay, Dictionary<long, CoordinatesModel> coordinatesCache)
        {
            var ways = new Way[waysSize];
            for (int i = 0; i < waysSize; i++)
            {
                for (int j = 0; j < nodesInAWay; j++)
                {
                    coordinatesCache.Add(_fixture.Create<int>(), _fixture.Create<CoordinatesModel>());
                }

                ways[i] = _fixture
                    .Build<Way>()
                    .With(x => x.Nodes, coordinatesCache
                        .TakeLast(nodesInAWay)
                        .Select(y => y.Key)
                        .ToArray())
                    .With(x => x.Tags,
                        new TagsCollection(_tags[_random.Next(_tags.Length - 1)], new Tag("name", _fixture.Create<string>())))
                    .Create();
            }

            return ways;
        }
    }
}
