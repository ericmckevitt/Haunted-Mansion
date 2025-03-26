using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PossessionManager : MonoBehaviour
{
    public float possessionInterval = 30f;
    public Transform player;
    private List<GameObject> possessableObjects = new List<GameObject>();
    private PossessedObject currentPosessedScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("Possessable");
        possessableObjects.AddRange(foundObjects);

        if (possessableObjects.Count == 0)
        {
            Debug.LogError("No objects with the tag 'Possessable' found in the scene.");
            return;
        }

        StartCoroutine(SwapPossessedObject()); // do this every minute (posessionInterval)
    }

    IEnumerator SwapPossessedObject()
    {
        while (true)
        {
            if (currentPosessedScript != null)
            {
                currentPosessedScript.StopPosession();
            }

            GameObject newPossessedObject = possessableObjects[Random.Range(0, possessableObjects.Count)];

            // Ensure object has a PossessedObject script or add one
            currentPosessedScript = newPossessedObject.GetComponent<PossessedObject>();
            if (currentPosessedScript == null)
            {
                currentPosessedScript = newPossessedObject.AddComponent<PossessedObject>();
            }

            currentPosessedScript.StartPosession(player);

            yield return new WaitForSeconds(possessionInterval);
        }
    } 
}
