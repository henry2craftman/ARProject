using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restaurant : MonoBehaviour
{
    [SerializeField] GPSData gps;
    public GPSData Gps { get { return gps; } set { gps = value; } }

    void Start()
    {
        
    }
}
