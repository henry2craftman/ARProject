//#define UNITY_EDITOR
#define UNITY_ANDROID

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

// ����: ARRay�� �߻��Ͽ� ����� Plane�� ������ �޾� �� ��ġ�� Indicator�� ��ġ��Ų��.
// �ʿ�Ӽ�: Indicator GameObject
// ����2: ��ġ�Է��� ������ Cyberpunk car�� ������ �ٴ� ���� ��ġ��Ų��.
// �ʿ�Ӽ�2: Cyberpunk car
public class ObjectManager : MonoBehaviour
{

    // �ʿ�Ӽ�: Indicator GameObject
    public GameObject indicator;
    ARRaycastManager raycastManager;

    // �ʿ�Ӽ�2: Cyberpunk car
    public GameObject displayObject;
    public GameObject editorPlane;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        displayObject.SetActive(false);
#if UNITY_EDITOR
        editorPlane.SetActive(true);
#elif UNITY_ANDROID
        editorPlane.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlane();
    }

    // ����: ARRay�� �߻��Ͽ� ����� Plane�� ������ �޾� �� ��ġ�� Indicator�� ��ġ��Ų��.
    void DetectPlane()
    {

#if UNITY_EDITOR
        if(Input.GetMouseButton(0))
        {
            // �� ��ũ�� �����̽��� Ŭ���� �������� ���� ���̸� �߻��Ѵ�.
            // 1. ��ũ���� ��ġ�� ��ǥ
            Vector3 touchPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
            // 2. ��ũ���� ��ġ�� ��ǥ�� World Point��
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touchPos);
            // 3. ������ ����
            Vector3 direction = (touchWorldPos - transform.position).normalized;
            // 4. �ش� �������� ���̸� ���.
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, direction * 100, Color.red, 0.5f);
                // 5. ���̿� �浹�� ������Ʈ�� �ٴ��̶��, �ٴ��� Ư�� ������ displayObject�� ��ġ��Ų��.
                if(hit.collider.name.Contains("Plane"))
                {
                    Debug.DrawRay(transform.position, direction * 100, Color.green, 0.5f);

                    displayObject.transform.position = hit.point;
                    displayObject.transform.rotation = hit.transform.rotation;
                    displayObject.SetActive(true);
                }
                else
                {
                    displayObject.SetActive(false);
                }
            }
        }

#elif UNITY_ANDROID
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

        TouchScreen();
#endif
    }

    // ����2: ��ġ�Է��� ������ Cyberpunk car�� ������ �ٴ� ���� ��ġ��Ų��.
    void TouchScreen()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                // indicator�� plane�� ã����
                if(indicator.activeSelf)
                {
                    displayObject.transform.position = indicator.transform.position;
                    displayObject.transform.rotation = indicator.transform.rotation;
                    displayObject.SetActive(true);
                }
            }
        }
    }
}
