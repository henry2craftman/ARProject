using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 
public class CompassManager : MonoBehaviour
{
    [SerializeField] RectTransform rootObj;
    [SerializeField] TextMeshProUGUI angleText;
    float angle;

    // Start is called before the first frame update
    void Start()
    {
        Input.compass.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        rootObj.rotation = Quaternion.Euler(0, 0, Input.compass.trueHeading);

        angle = Mathf.RoundToInt(Input.compass.trueHeading);

        angleText.text = angle.ToString() + "вк";
    }
}
