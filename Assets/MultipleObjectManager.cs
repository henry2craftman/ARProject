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
    }

    void OnImageTrackedEvent(ARTrackedImagesChangedEventArgs args)
    {
        logText.text = "";

        foreach (ARTrackedImage trackedImage in args.added)
        {
            string imageName = trackedImage.referenceImage.name;

            logText.text = logText.text + "\n" + imageName;

            GameObject prefab = Resources.Load<GameObject>(imageName);

            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation);

                logText.text = logText.text + "\n" + obj.name + "이 추가 되었습니다.";
            }
        }
    }
}
