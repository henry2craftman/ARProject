using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using CesiumForUnity;
using UnityEngine.UI;

// 목적: GPS 위치 사용에 대한 권한 확인 및 GPS 위치 표시
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
    /// latitude를 X로, Longtitude를 Y로 정한 후, 스케일 요소를 통해 distance를 100m 단위로 구합니다. 
    /// 파라미터 값이 소수점 6째 자리까지 존재해야 함.(예: 위도 37.513856 일때, 3.856(100m 단위로 변환)
    /// 최종적으로 3.856 * 100 -> m단위로 값을 반환
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    /// <returns></returns>
    private float Distance(double startX, double startY, double endX, double endY)
    {
        // 예시) 경도 1도/100km, 0.1/10km, 0.01/1km, 0.001/100m, 0.0001/10m, 0.00001/1m, 0.000001/10cm
        string tempStr = startX.ToString(); // ex) 위도 37.513856
        tempStr = tempStr.Substring(5, 4); // 5번째 index 부터 4개의 문자열 추출 3856
        float startPointX = float.Parse(tempStr) * 0.001f; // 3856 -> 3.856(100m 단위로 변환)

        tempStr = startY.ToString(); // 경도 127.029549
        tempStr = tempStr.Substring(6, 4); // 6번째 index 부터 4개의 문자열 추출 9549
        float startPointY = float.Parse(tempStr) * 0.001f; // 9549 -> 9.549(100m 단위로 변환)

        tempStr = endX.ToString();
        tempStr = tempStr.Substring(5, 4);
        float endPointX = float.Parse(tempStr) * 0.001f;

        tempStr = endY.ToString();
        tempStr = tempStr.Substring(6, 4);
        float endPointY = float.Parse(tempStr) * 0.001f;

        float distance = Mathf.Sqrt(Mathf.Pow(endPointX - startPointX, 2) + Mathf.Pow(endPointY - startPointY, 2));

        return distance * 100; // m단위로 변환
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

    // Location Manager의 restaurantDB안의 restaurant와으 거리를 계산
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
            // ex) 위도 37.513856, 경도 127.029549
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

    // GPS 위치정보 사용 허가 받기
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

    // GPS 초기화
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
