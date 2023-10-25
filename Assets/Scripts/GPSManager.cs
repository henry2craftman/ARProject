using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

// ����: GPS ��ġ ��뿡 ���� ���� Ȯ�� �� GPS ��ġ ǥ��
public class GPSManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI latitudeText;
    [SerializeField] TextMeshProUGUI longtitudeText;
    [SerializeField] float range = 10f;
    double latitutde = 0;
    double longtitude = 0;
    bool isReceived = false;
    int waitTime = 0;
    LocationManager locationManager;
    

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        locationManager = FindObjectOfType<LocationManager>();
    }

    private void Start()
    {
        CalculateDistance();

        StartCoroutine(TurnOnGPS());
    }

    /// <summary>
    /// latitude�� X��, Longtitude�� Y�� ���� ��, ������ ��Ҹ� ���� distance�� 100m ������ ���մϴ�. 
    /// �Ķ���� ���� �Ҽ��� 6° �ڸ����� �����ؾ� ��.(��: ���� 37.513856 �϶�, 3.856(100m ������ ��ȯ)
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

        return distance;
    }

    IEnumerator TurnOnGPS()
    {
        InitializePermission();

        SetGPSInfo();

        InitializeGPS();

        while(isReceived)
        {
            SetGPSInfo();

            CalculateDistance();

            yield return new WaitForSeconds(1);

            waitTime++;
        }
    }

    // Location Manager�� restaurantDB���� restaurant���� �Ÿ��� ���
    private void CalculateDistance()
    {
        List<GPSData> restaurantDB = locationManager.RestaurantDB;

        foreach (GPSData gps in  restaurantDB)
        {
            // TODO: Distance �Լ� �����ؾ���
            float distance = Distance(37.513856, 127.029549, gps.latitude, gps.longtitutitude);

            if(distance * 100 <= range)
            {
                print(gps.restaurantName + " / " + distance * 100);
                locationManager.SetActiveObject(gps.restaurantName, true);
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
        latitutde = location.latitude;
        longtitude = location.longitude;

        latitudeText.text = "Latitude: " + latitutde.ToString();
        longtitudeText.text = "Longtitude: " + longtitude.ToString();
    }
}
