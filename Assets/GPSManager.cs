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
    float latitutde = 0;
    float longtitude = 0;
    bool isReceived = false;
    int waitTime = 0;

    private void Start()
    {
        StartCoroutine(TurnOnGPS());
    }

    IEnumerator TurnOnGPS()
    {
        InitializePermission();

        SetGPSInfo();

        InitializeGPS();

        while(isReceived)
        {
            SetGPSInfo();

            yield return new WaitForSeconds(1);

            waitTime++;
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
