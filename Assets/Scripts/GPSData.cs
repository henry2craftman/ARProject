using System;

[Serializable]
public class GPSData
{
    public string restaurantName;
    public double latitude;
    public double longtitude;
    public double altitude;
    public bool isCaptued;

    public GPSData(string restaurantName, double latitude, double longtitude, double altitude, bool isCaptued)
    {
        this.restaurantName = restaurantName;
        this.latitude = latitude;
        this.longtitude = longtitude;
        this.altitude = altitude;
        this.isCaptued = isCaptued;
    }
}
