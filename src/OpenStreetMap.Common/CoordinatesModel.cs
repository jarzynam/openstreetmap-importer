namespace OpenStreetMap.Common;

public struct CoordinatesModel
{
    public CoordinatesModel(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; init; }
    public double Longitude { get; init; }
}