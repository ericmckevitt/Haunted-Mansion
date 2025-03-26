using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightManager : MonoBehaviour
{

    private List<GameObject> lightObjects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("Light");
        lightObjects.AddRange(foundObjects);
        Debug.Log($"{lightObjects.Count} lights found.");

        foreach (GameObject light in lightObjects) {
            Transform lightTransform = light.transform;
            for (int i = 0; i < lightTransform.childCount; i++) {
                lightTransform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
