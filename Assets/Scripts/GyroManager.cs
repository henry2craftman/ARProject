using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroManager : MonoBehaviour
{
    [SerializeField] Transform compass;
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
            compass.rotation = Quaternion.Euler(0, -gyroRotation.eulerAngles.y, 0);
        }
    }
}
