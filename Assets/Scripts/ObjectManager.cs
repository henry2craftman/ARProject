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
// ����3: Ŭ��(��ġ) ���¶�� ������Ʈ�� ��ȭ�� ��ŭ Y������ ȸ����Ų��.
// �ʿ�Ӽ�3: ���콺 �̵� ��ȭ�� ����
// ����4: �������� ������ �ʹ�.
public class ObjectManager : MonoBehaviour
{

    // �ʿ�Ӽ�: Indicator GameObject
    public GameObject indicator;
    ARRaycastManager raycastManager;

    // �ʿ�Ӽ�2: Cyberpunk car
    public GameObject displayObject;
    public GameObject editorPlane;

    // �ʿ�Ӽ�3: ���콺 ���� ������ ����, ���콺 �̵� ��ȭ�� ����, ȸ�� �ӵ� ���� ����
    Vector3 prevPos;
    Vector3 deltaPos;
    public float rotationScaleMultiplier = 0.1f;
    Vector3 startPos;
    Vector3 endPos;
    public Transform pokeball;
    Vector3 originPokeballPos;
    public float throwPowerMultiplier = 0.005f;
    public float pokeballResetTime = 3;
    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        displayObject.SetActive(false);
        originPokeballPos = pokeball.transform.position;
#if UNITY_EDITOR
        editorPlane.SetActive(true);
#elif UNITY_ANDROID
        editorPlane.SetActive(false);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
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

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
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

            prevPos = Input.mousePosition;
        }
        // ����3: Ŭ��(��ġ) ���¶�� ������Ʈ�� ��ȭ�� ��ŭ Y������ ȸ����Ų��.
        else if (Input.GetMouseButton(0)) 
        {
            deltaPos = (Input.mousePosition - prevPos);
            
            // y������ ��ȭ�� ��ŭ ȸ����Ų��.
            displayObject.transform.Rotate(transform.up, -deltaPos.normalized.x * rotationScaleMultiplier);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, direction * 100, Color.red, 0.5f);
                // 5. ���̿� �浹�� ������Ʈ�� �ٴ��̶��, �ٴ��� Ư�� ������ displayObject�� ��ġ��Ų��.
                if (hit.collider.name == "Pokeball" && pokeball != null)
                {
                    pokeball.transform.position = new Vector3(hit.point.x, hit.point.y, pokeball.transform.position.z);
                }
            }
        }
        // ����4: ���Ϻ��� �巡��&������� ������ �ʹ�.
        else if (Input.GetMouseButtonUp(0))
        {
            if (pokeball == null)
                return;

            endPos = Input.mousePosition;
            Vector3 deltaPos = endPos - startPos;
            float throwPower = deltaPos.magnitude;

            // ����4: �������� ������ �ʹ�.
            pokeball.GetComponent<Rigidbody>().useGravity = true;
            pokeball.GetComponent<Rigidbody>().AddForce(direction * throwPower * throwPowerMultiplier, ForceMode.Impulse);
            Invoke("ResetPokeball", pokeballResetTime);
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
            // ����3: Ŭ��(��ġ) ���¶�� ������Ʈ�� ��ȭ�� ��ŭ Y������ ȸ����Ų��.
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 deltaPos = touch.deltaPosition;

                // y������ ��ȭ�� ��ŭ ȸ����Ų��.
                displayObject.transform.Rotate(transform.up, -deltaPos.normalized.x * rotationScaleMultiplier);
            }
        }
    }

    void ResetPokeball()
    {
        pokeball.position = originPokeballPos;
        pokeball.rotation = Quaternion.identity;
        pokeball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        pokeball.GetComponent<Rigidbody>().angularVelocity= Vector3.zero;
        pokeball.GetComponent<Rigidbody>().useGravity = false;
    }
}
