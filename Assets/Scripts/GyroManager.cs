using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GyroManager : MonoBehaviour
{
    [SerializeField] Transform compass;
    [SerializeField] TextMeshProUGUI gyroText;
    private bool gyroEnabled;
    private Gyroscope gyro;
    Quaternion gyroRotation = new Quaternion();
    public Quaternion GyroRotation { get { return gyroRotation; } }
    

    private void Awake()
    {
        gyroEnabled = EnableGyro();
    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (gyroEnabled)
        {
            gyroRotation = gyro.attitude;

            //// TODO: 수정 필요
            //compass.rotation = Quaternion.Euler(0, -heading, 0);
        }
    }
}
