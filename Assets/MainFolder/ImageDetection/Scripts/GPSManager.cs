using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using CesiumForUnity;
using UnityEngine.UI;

// ����: GPS ��ġ ��뿡 ���� ���� Ȯ�� �� GPS ��ġ ǥ��
public class GPSManager : MonoBehaviour
{
    [SerializeField] CesiumForUnity.CesiumGeoreference georeference;
    [SerializeField] TextMeshProUGUI latitudeText;
    [SerializeField] TextMeshProUGUI longtitudeText;
    [SerializeField] float range = 10f;
    [SerializeField] Text debugText;
    double latitude = 0;
    double longtitude = 0;
    private float altitude;
    bool isReceived = false;
    int waitTime = 0;
    LocationManager locationManager;
    GyroManager gyroManager;

    private void Awake()
    {
        locationManager = FindObjectOfType<LocationManager>();
        gyroManager = FindObjectOfType<GyroManager>();
        georeference = FindObjectOfType<CesiumGeoreference>();
    }

    private void Start()
    {
        StartCoroutine(TurnOnGPS());
    }

    /// <summary>
    /// latitude�� X��, Longtitude�� Y�� ���� ��, ������ ��Ҹ� ���� distance�� 100m ������ ���մϴ�. 
    /// �Ķ���� ���� �Ҽ��� 6° �ڸ����� �����ؾ� ��.(��: ���� 37.513856 �϶�, 3.856(100m ������ ��ȯ)
    /// ���������� 3.856 * 100 -> m������ ���� ��ȯ
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    /// <returns></returns>
    private float Distance(double startX, double startY, double endX, double endY)
    {
        // ����) �浵 1��/100km, 0.1/10km, 0.01/1km, 0.001/100m, 0.0001/10m, 0.00001/1m, 0.000001/10cm
        string tempStr = startX.ToString(); // ex) ���� 37.513856
        tempStr = tempStr.Substring(5, 4); // 5��° index ���� 4���� ���ڿ� ���� 3856
        float startPointX = float.Parse(tempStr) * 0.001f; // 3856 -> 3.856(100m ������ ��ȯ)

        tempStr = startY.ToString(); // �浵 127.029549
        tempStr = tempStr.Substring(6, 4); // 6��° index ���� 4���� ���ڿ� ���� 9549
        float startPointY = float.Parse(tempStr) * 0.001f; // 9549 -> 9.549(100m ������ ��ȯ)

        tempStr = endX.ToString();
        tempStr = tempStr.Substring(5, 4);
        float endPointX = float.Parse(tempStr) * 0.001f;

        tempStr = endY.ToString();
        tempStr = tempStr.Substring(6, 4);
        float endPointY = float.Parse(tempStr) * 0.001f;

        float distance = Mathf.Sqrt(Mathf.Pow(endPointX - startPointX, 2) + Mathf.Pow(endPointY - startPointY, 2));

        return distance * 100; // m������ ��ȯ
    }

    IEnumerator TurnOnGPS()
    {
        yield return InitializePermission();

        yield return InitializeGPS();

        while (isReceived)
        {
            SetGPSInfo();

            UpdateGeospatialCretor();

            //CalculateDistance();

            yield return new WaitForSeconds(1);

            waitTime++;
        }
    }

    private void UpdateGeospatialCretor()
    {
        georeference.latitude = latitude;
        georeference.longitude = longtitude;
        georeference.height = altitude;

        debugText.text += $"latitude, longitude, height\n{georeference.latitude}, {georeference.longitude}, {georeference.height}";
    }

    // Location Manager�� restaurantDB���� restaurant���� �Ÿ��� ���
    private void CalculateDistance()
    {
        List<GPSData> restaurantDB = locationManager.RestaurantDB;
        //longtitudeText.text += "\nrestaurantDB count: " + restaurantDB.Count;
        //longtitudeText.text += "\nlatitude: " + latitude + " / longtitude: " + longtitude;

        if (latitude == 0 || longtitude == 0)
        {
            return;
        }

        foreach (GPSData gps in  restaurantDB)
        {
            // ex) ���� 37.513856, �浵 127.029549
            float distance = Distance(latitude, longtitude, gps.latitude, gps.longtitude);

            if(distance <= range)
            {
                //longtitudeText.text += "\n" + gps.restaurantName + " is within " + (distance) + "m";
                bool isActive = locationManager.SetActiveObject(gps.restaurantName, true);
                longtitudeText.text += "\nGyroRotationY: " + gyroManager.GyroRotation.eulerAngles.z;

                // gyroManager.GyroRotation.eulerAngles.y = roll
                float newX = Mathf.Cos(gyroManager.GyroRotation.eulerAngles.z) * distance;
                float newY = Mathf.Sin(gyroManager.GyroRotation.eulerAngles.z) * distance;
                longtitudeText.text += "\n" + gps.restaurantName + " is Active: " + isActive + " / newX: " + newX + " / newY: " + newY;

                print(newX + " / " + newY);
                locationManager.LocateObject(gps.restaurantName, newX, newY);
            }
            else
            {
                locationManager.SetActiveObject(gps.restaurantName, false);
            }
        }
    }

    // GPS ��ġ���� ��� �㰡 �ޱ�
    IEnumerator InitializePermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                latitudeText.text = "Please accept GPS permission.\nThe application will be quit in 3 seconds";
                longtitudeText.text = "";

                yield return new WaitForSeconds(3);

                Application.Quit();
            }
        }

        if (!Input.location.isEnabledByUser)
        {
            latitudeText.text = "GPS is not allowed";
            yield break;
        }
        else
        {
            latitudeText.text = "GPS is allowed";
        }
    }

    // GPS �ʱ�ȭ
    IEnumerator InitializeGPS()
    {
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                latitudeText.text = "GPS is running!";
                break;
            }

            yield return new WaitForSeconds(1);
            waitTime++;
            latitudeText.text = "GPS Initializing: " + waitTime + "s";
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            latitudeText.text = "GPS initialization failed";
            isReceived = false;
        }
        else
        {
            isReceived = true;
            waitTime = 0;
        }
    }

    private void SetGPSInfo()
    {
        Input.location.Start();

        LocationInfo location = Input.location.lastData;
        latitude = location.latitude;
        longtitude = location.longitude;
        altitude = location.altitude;

        latitudeText.text = "Latitude: " + latitude.ToString();
        longtitudeText.text = "Longtitude: " + longtitude.ToString();
    }


}
