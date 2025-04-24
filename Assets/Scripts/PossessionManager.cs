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

            // Filter list to only include nearby objects
            List<GameObject> nearbyObjects = new List<GameObject>();
            foreach (GameObject obj in possessableObjects)
            {
                if (Vector3.Distance(player.position, obj.transform.position) <= 20f)
                {
                    nearbyObjects.Add(obj);
                }
            }

            if (nearbyObjects.Count == 0)
            {
                Debug.LogWarning("No possessable objects within 15 units of the player.");
                yield return new WaitForSeconds(possessionInterval);
                continue;
            }

            GameObject newPossessedObject = nearbyObjects[Random.Range(0, nearbyObjects.Count)];

            // Ensure object has a PossessedObject script
            currentPosessedScript = newPossessedObject.GetComponent<PossessedObject>();
            if (currentPosessedScript == null)
            {
                currentPosessedScript = newPossessedObject.AddComponent<PossessedObject>();
            }

            // Add AudioSource if needed
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


    // Spawn more objects on win cond
    public void SpawnExtraPossessions(int count)
    {
        // 1. Build a nearby‑and‑not‑already‑possessed list
        List<GameObject> nearby = new List<GameObject>();

        foreach (GameObject obj in possessableObjects)
        {
            if (obj == currentPosessedScript?.gameObject) continue;              // skip the one already possessed
            if (Vector3.Distance(player.position, obj.transform.position) > 20f) continue; // too far away?

            nearby.Add(obj);
        }

        // 2. If nothing to spawn, bail early
        if (nearby.Count == 0) return;

        // 3. Spawn up to <count> new possessions
        for (int i = 0; i < count && nearby.Count > 0; i++)
        {
            // Pick a random candidate and remove it from the list so we never pick it again
            int index          = Random.Range(0, nearby.Count);
            GameObject obj     = nearby[index];
            nearby.RemoveAt(index);

            // Get or add the PossessedObject script
            PossessedObject script = obj.GetComponent<PossessedObject>() ??
                                    obj.AddComponent<PossessedObject>();

            // Get or add the AudioSource
            AudioSource audio = obj.GetComponent<AudioSource>();
            if (audio == null)
            {
                audio = obj.AddComponent<AudioSource>();
                audio.loop          = true;
                audio.playOnAwake   = false;
                audio.spatialBlend  = 1f;
                audio.minDistance   = 2f;
                audio.maxDistance   = 20f;
            }

            audio.clip        = scrapingSound;
            script.scapeSound = scrapingSound;

            script.StartPosession(player);
        }
    }
    
}
