using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    [SerializeField] GameObject restaurantPrefab;
    [SerializeField] List<GPSData> restaurantDB = new List<GPSData>();
    public List<GPSData> RestaurantDB { get { return restaurantDB; } }
    [SerializeField] List<Transform> restaurants = new List<Transform>();

    void Awake()
    {
        foreach(GPSData gps in restaurantDB)
        {
            GameObject restaurantObj = Instantiate(restaurantPrefab, transform);
            restaurantObj.name = gps.restaurantName;
            restaurantObj.GetComponent<Restaurant>().Gps = gps;
            restaurantObj.SetActive(false);

            restaurants.Add(restaurantObj.transform);
        }
    }

    public void SetActiveObject(string name, bool isActive)
    {
        foreach(Transform restaurant in restaurants)
        {
            if (restaurant.name.Equals(name))
            {
                restaurant.gameObject.SetActive(isActive);
            }
        }

    }
}
