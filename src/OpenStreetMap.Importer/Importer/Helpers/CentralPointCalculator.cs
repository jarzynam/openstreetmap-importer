using System;
using System.Collections.Generic;
using System.Linq;
using OpenStreetMap.Common;

namespace OpenStreetMap.Importer.Importer.Helpers
{
    public static class CentralPointCalculator
    {
        public static CoordinatesModel Calculate(List<CoordinatesModel> coordinate)
        {
            if (coordinate.Count == 1)
                return coordinate.Single();

            double accumulatedArea = 0.0f;
            double centerLatitude = 0.0f;
            double centerLongitude = 0.0f;

            for (int i = 0, j = coordinate.Count - 1; i < coordinate.Count; j = i++)
            {
                double temp = coordinate[i].Latitude * coordinate[j].Longitude - coordinate[j].Latitude * coordinate[i].Longitude;
                accumulatedArea += temp;
                centerLatitude += (coordinate[i].Latitude + coordinate[j].Latitude) * temp;
                centerLongitude += (coordinate[i].Longitude + coordinate[j].Longitude) * temp;
            }

            if (Math.Abs(accumulatedArea) < 1E-7f)
            {
                return coordinate.FirstOrDefault();
            }

            accumulatedArea *= 3f;
            return new CoordinatesModel { Latitude = centerLatitude / accumulatedArea, Longitude = centerLongitude / accumulatedArea };
        }
    }
}