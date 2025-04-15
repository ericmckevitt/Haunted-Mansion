using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PossessionManager : MonoBehaviour
{
    public float possessionInterval = 30f;
    public Transform player;
    public AudioClip scrapingSound;
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

            // 🔊 Dynamically add AudioSource if needed
            AudioSource audioSource = newPossessedObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = newPossessedObject.AddComponent<AudioSource>();
                audioSource.clip = scrapingSound;
                audioSource.loop = true;
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f;
                audioSource.minDistance = 2f;
                audioSource.maxDistance = 20f;
            }
            currentPosessedScript.scapeSound = scrapingSound;
            
            currentPosessedScript.StartPosession(player);

            yield return new WaitForSeconds(possessionInterval);
        }
    } 
}
