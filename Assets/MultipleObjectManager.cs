using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MultipleObjectManager : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager imageManager;
    [SerializeField] TextMeshProUGUI logText;

    private void Start()
    {
        imageManager = GetComponent<ARTrackedImageManager>();

        imageManager.trackedImagesChanged += OnImageTrackedEvent;

        logText.text = "";
    }

    void OnImageTrackedEvent(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {
            string imageName = trackedImage.referenceImage.name;
            //logText.text = logText.text + "\n" + imageName; // OK

            GameObject prefab = Resources.Load<GameObject>(imageName);

            if (prefab != null)
            {
                if (trackedImage.transform.childCount < 1)
                {
                    GameObject obj = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation);
                    obj.transform.SetParent(trackedImage.transform);
                    logText.text = logText.text + "\n" + obj.name + "is added.";
                }
            }
        }

        foreach (ARTrackedImage trackedImage in args.updated)
        {
            if (trackedImage.transform.childCount > 0)
            {
                trackedImage.transform.gameObject.SetActive(true);
                trackedImage.transform.GetChild(0).position = trackedImage.transform.position;
                trackedImage.transform.GetChild(0).rotation = trackedImage.transform.rotation;
                // trackedImage.transform.GetChild(0).gameObject -> Canvas로 출력됨
            }
        }

        foreach (ARTrackedImage trackedImage in args.removed)
        {
            // TODO: trackedImage의 child 비활성화 필요 -> Log로 찍어볼 것
            if (trackedImage.transform.childCount > 0)
            {
                trackedImage.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
