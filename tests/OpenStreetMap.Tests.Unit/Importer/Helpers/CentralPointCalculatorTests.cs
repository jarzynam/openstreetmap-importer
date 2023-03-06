using FluentAssertions;
using OpenStreetMap.Common;
using OpenStreetMap.Importer.Importer.Helpers;

namespace OpenStreetMap.Tests.Unit.Importer.Helpers
{
    [TestClass]
    public class CentralPointCalculatorTests
    {

        [TestMethod]
        public void Calculate_ShouldReturnFirstNull_WhenIsEmpty()
        {
            // arrange
            var points = new List<CoordinatesModel>
            {
            };
            CoordinatesModel expectedCenter = default;

            // act
            var actualCenter = CentralPointCalculator.Calculate(points);

            // assert
            actualCenter.Should().Be(expectedCenter);
        }

        [TestMethod]
        [DataRow(10, 30)]
        [DataRow(5, 3)]
        [DataRow(0, 0)]
        [DataRow(0.1, 0.0000004)]
        [DataRow(-80000, 0)]
        public void Calculate_ShouldReturnFirstPoint_WhenIsAPoint(double lat1, double long1)
        {
            // arrange
            var points = new List<CoordinatesModel>
            {
                new(latitude: lat1, longitude: long1),
            };
            var expectedCenter = points[0];

            // act
            var actualCenter = CentralPointCalculator.Calculate(points);

            // assert
            actualCenter.Should().Be(expectedCenter);
        }


        [TestMethod]
        [DataRow(10, 30, 3, 5)]
        [DataRow(5, 3, 20, 30000)]
        [DataRow(0, 0, 0, 0)]
        [DataRow(0.1, 0.0000004, 20000000, 300000000000)]
        [DataRow(-80000, 0, 0, -6660)]
        public void Calculate_ShouldReturnFirstPoint_WhenIsALine(double lat1, double long1, double lat2, double long2)
        {
            // arrange
            var points = new List<CoordinatesModel>
            {
                new(latitude: lat1, longitude: long1),
                new(latitude: lat2, longitude: long2)
            };
            var expectedCenter = points[0];

            // act
            var actualCenter = CentralPointCalculator.Calculate(points);

            // assert
            actualCenter.Should().Be(expectedCenter);
        }


        [TestMethod]
        [DataRow(7.13, 14.23, 10, 30, 3, 5, 8,8)]
        [DataRow(-4.82, 9.9, -18.30, 25.7, 4, 0, 0,4)]
        [DataRow(0, 0, 0, 0, 0, 0,0,0)]
        public void Calculate_ShouldReturnCenterPoint_WhenTriangle(double expLat, double expLong, params double[] input)
        {
            // arrange
            var points = TransformInputCoordinates(input);

            var expectedCenter = new CoordinatesModel(expLat, expLong);

            // act
            var actualCenter = CentralPointCalculator.Calculate(points);

            // assert
            actualCenter.Longitude.Should().BeApproximately(expectedCenter.Longitude, 0.2);
            actualCenter.Latitude.Should().BeApproximately(expectedCenter.Latitude, 0.2);
        }

        [TestMethod]
        [DataRow(1.5,1.5,/*p1*/ 1, 1/*p2*/, 1,2, /*p3*/ 2, 2,/*p4*/ 2,1, /*p5*/ 3,1)]
        [DataRow(19.72, -14.56,/*p1*/ 18, 0/*p2*/, 5, 3, /*p3*/ 23, -2,/*p4*/ 23, -41, /*p5*/ 13, -1)]
        [DataRow(0, 0,/*p1*/ 0, 0/*p2*/, 0, 0, /*p3*/ 0, 0,/*p4*/ 0, 0, /*p5*/ 0, 0)]


        public void Calculate_ShouldReturnCenterPoint_WhenPolygon(double expLat, double expLong, params double[] input)
        {
            // arrange
            var points = TransformInputCoordinates(input);

            var expectedCenter = new CoordinatesModel(expLat, expLong);

            // act
            var actualCenter = CentralPointCalculator.Calculate(points);

            // assert
            actualCenter.Longitude.Should().BeApproximately(expectedCenter.Longitude, 0.2);
            actualCenter.Latitude.Should().BeApproximately(expectedCenter.Latitude, 0.2);
        }

        private static List<CoordinatesModel> TransformInputCoordinates(double[] input)
        {
            var points = new List<CoordinatesModel>(input.Length / 2);

            for (int i = 0; i < input.Length; i += 2)
            {
                points.Add(new CoordinatesModel(input[i], input[i + 1]));
            }

            return points;
        }
    }
}