using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

// ����: ARRay�� �߻��Ͽ� ����� Plane�� ������ �޾� �� ��ġ�� Indicator�� ��ġ��Ų��.
// �ʿ�Ӽ�: Indicator GameObject
public class ObjectManager : MonoBehaviour
{

    // �ʿ�Ӽ�: Indicator GameObject
    public GameObject indicator;
    ARRaycastManager raycastManager;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlane();
    }

    // ����: ARRay�� �߻��Ͽ� ����� Plane�� ������ �޾� �� ��ġ�� Indicator�� ��ġ��Ų��.
    void DetectPlane()
    {
        // ��ũ�� �߾��� ��ġ
        Vector2 screenCenter = new Vector2 (Screen.width * 0.5f, Screen.height * 0.5f);

        // �浹�� ������ ������ ��� ����
        List<ARRaycastHit> hitInfo = new List<ARRaycastHit>();

        // ���̸� �߻��Ͽ� �΋H�� �÷����� ������ hitInfo�� ����
        if(raycastManager.Raycast(screenCenter, hitInfo, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            indicator.SetActive(true);
            indicator.transform.position = hitInfo[0].pose.position;
            indicator.transform.rotation = hitInfo[0].pose.rotation;
        }
        else
        {
            indicator.SetActive(false);
        }

    }
}
