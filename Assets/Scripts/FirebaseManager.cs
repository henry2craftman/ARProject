using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using System;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEditor;

public class FirebaseManager : MonoBehaviour
{
    [SerializeField] string dbURL;
    [SerializeField] GPSDataList data = new GPSDataList();
    public GPSDataList Data {  get { return data; } }
    [Serializable]
    public class GPSDataList
    {
        public List<GPSData> gPS = new List<GPSData>();
    }
    public bool isReceived = false;

    private void Awake()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(dbURL);

        SendData();

        RequestData();
    }

    private void RequestData()
    {
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        string json = "";

        dbRef.GetValueAsync().ContinueWith(LoadFunction);

        void LoadFunction(Task<DataSnapshot> task)
        {
            if (task.IsFaulted)
            {
                Debug.Log("Data request is faulted!");
            }
            else if(task.IsCanceled)
            {
                Debug.Log("Data request is canceled!");
            }
            else if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                foreach(var data in snapshot.Children)
                {
                    json = data.GetRawJsonValue();

                    print(json);
                }

                data = JsonUtility.FromJson<GPSDataList>(json);
                isReceived = true;
            }
        }
    }

    private void SendData()
    {
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        GPSData gps1 = new GPSData("Homeplus", 37.513647, 127.030041, 55, false);
        GPSData gps2 = new GPSData("Yangjoo Wedding", 37.513301, 127.028982, 55, false);

        GPSDataList gpsList = new GPSDataList();
        gpsList.gPS.Add(gps1);
        gpsList.gPS.Add(gps2);

        string dataList = JsonUtility.ToJson(gpsList);

        dbRef.Child("GPSList").SetRawJsonValueAsync(dataList);
    }
}
