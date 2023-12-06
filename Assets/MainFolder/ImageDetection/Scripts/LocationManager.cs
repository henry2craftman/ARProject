using CesiumForUnity;
using Google.XR.ARCoreExtensions;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    [SerializeField] GameObject restaurantPrefab;
    [SerializeField] List<GPSData> restaurantDB = new List<GPSData>();
    public List<GPSData> RestaurantDB { get { return restaurantDB; } }
    [SerializeField] List<Transform> restaurants = new List<Transform>();
    [SerializeField] UnityEngine.UI.Text debugTxt;
    FirebaseManager fbManager;

    void Awake()
    {
        fbManager = FindAnyObjectByType<FirebaseManager>();

        StartCoroutine(CreateObjs());
    }

    IEnumerator CreateObjs()
    {
        yield return new WaitUntil(() => fbManager.isReceived);

        debugTxt.text = "data is received";

        FirebaseManager.GPSDataList dataList = fbManager.Data;

        foreach (GPSData gps in dataList.gPS)
        {
            GameObject restaurantObj = Instantiate(restaurantPrefab, transform);
            restaurantObj.name = gps.restaurantName;

            Restaurant info = restaurantObj.GetComponent<Restaurant>();
            info.Gps = gps;
            info.nameTxt.text = gps.restaurantName;


            //restaurantObj.SetActive(false);

            ARGeospatialCreatorAnchor anchorInfo = restaurantObj.AddComponent<ARGeospatialCreatorAnchor>();
            anchorInfo.Latitude = gps.latitude;
            anchorInfo.Longitude = gps.longtitude;
            anchorInfo.Altitude = gps.altitude;

            string gpsStr = string.Format("latitude: {0}, longtitude: {1}", anchorInfo.Latitude, anchorInfo.Longitude);
            debugTxt.text += $"\n{restaurantObj.name}: {gpsStr}";

            restaurants.Add(restaurantObj.transform);
        }
    }


    public bool SetActiveObject(string name, bool isActive)
    {
        foreach(Transform restaurant in restaurants)
        {
            if (restaurant.name.Equals(name))
            {
                restaurant.gameObject.SetActive(isActive);

                return true;
            }
        }

        return false;
    }

    public void LocateObject(string name, float xCoordinate, float yCoordinate)
    {
        foreach (Transform restaurant in restaurants)
        {
            if (restaurant.name.Equals(name))
            {
                restaurant.transform.position = new Vector3(xCoordinate, 0, yCoordinate);
            }
        }
    }
}
