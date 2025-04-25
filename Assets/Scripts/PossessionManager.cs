using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PossessionManager : MonoBehaviour
{
    [Header("Possession Timing")]
    public float possessionInterval = 10f;

    [Header("References")]
    public Transform player;
    public AudioClip scrapingSound;
    public StartMenu gameState;  // Drag your StartMenu here in the Inspector

    private List<GameObject> possessableObjects = new List<GameObject>();
    private PossessedObject currentPosessedScript;

    void Start()
    {
        // Fallback if you forgot to wire it up
        if (gameState == null)
            gameState = FindObjectOfType<StartMenu>();

        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("Possessable");
        possessableObjects.AddRange(foundObjects);

        if (possessableObjects.Count == 0)
        {
            Debug.LogError("No objects with the tag 'Possessable' found in the scene.");
            return;
        }

        StartCoroutine(SwapPossessedObject());
    }

    IEnumerator SwapPossessedObject()
    {
        while (true)
        {
            // 1) Stop the old possession
            if (currentPosessedScript != null)
            {
                currentPosessedScript.StopPosession();
                Debug.Log($"Stopping possession {currentPosessedScript.name}");
            }

            // 2) Pick a random nearby object
            List<GameObject> nearbyObjects = new List<GameObject>();
            foreach (GameObject obj in possessableObjects)
            {
                if (Vector3.Distance(player.position, obj.transform.position) <= 25f)
                    nearbyObjects.Add(obj);
            }

            if (nearbyObjects.Count == 0)
            {
                Debug.LogWarning("No possessable objects within 25 units of the player.");
                yield return new WaitForSeconds(possessionInterval);
                continue;
            }

            GameObject newObj = nearbyObjects[Random.Range(0, nearbyObjects.Count)];
            Debug.Log($"new possessed object = {newObj.name} (manager)");

            // 3) Ensure it has a PossessedObject component
            currentPosessedScript = newObj.GetComponent<PossessedObject>();
            if (currentPosessedScript == null)
                currentPosessedScript = newObj.AddComponent<PossessedObject>();

            // 4) Assign our shared references
            currentPosessedScript.gameState   = gameState;
            currentPosessedScript.scapeSound  = scrapingSound;

            // 5) Ensure AudioSource is configured
            AudioSource audioSource = newObj.GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = newObj.AddComponent<AudioSource>();
            audioSource.clip         = scrapingSound;
            audioSource.loop         = true;
            audioSource.playOnAwake  = false;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance  = 2f;
            audioSource.maxDistance  = 20f;

            // 6) Kick off the new possession and pass it forward
            currentPosessedScript.scapeSound = scrapingSound;
            currentPosessedScript.gameState  = gameState;       // ← ensure StartMenu is set
            currentPosessedScript.StartPosession(player);

            // 7) Wait before swapping again
            yield return new WaitForSeconds(possessionInterval);
        }
    }

    // ── UPDATED SpawnExtraPossessions ──
    public void SpawnExtraPossessions(int count)
    {
        // 1. Build a nearby-and-not-already-possessed list
        List<GameObject> nearby = new List<GameObject>();
        foreach (GameObject obj in possessableObjects)
        {
            if (obj == currentPosessedScript?.gameObject) continue;
            if (Vector3.Distance(player.position, obj.transform.position) > 20f) continue;
            nearby.Add(obj);
        }

        // 2. If nothing to spawn, bail early
        if (nearby.Count == 0) return;

        // 3. Spawn up to <count> new possessions
        for (int i = 0; i < count && nearby.Count > 0; i++)
        {
            int index      = Random.Range(0, nearby.Count);
            GameObject obj = nearby[index];
            nearby.RemoveAt(index);

            // Get or add the PossessedObject script
            PossessedObject script = obj.GetComponent<PossessedObject>()
                                    ?? obj.AddComponent<PossessedObject>();

            // Assign the shared references
            script.gameState  = gameState;
            script.scapeSound = scrapingSound;

            // Get or add the AudioSource
            AudioSource audio = obj.GetComponent<AudioSource>();
            if (audio == null)
                audio = obj.AddComponent<AudioSource>();
            audio.clip         = scrapingSound;
            audio.loop         = true;
            audio.playOnAwake  = false;
            audio.spatialBlend = 1f;
            audio.minDistance  = 2f;
            audio.maxDistance  = 20f;

            // Finally start possession
            script.StartPosession(player);
        }
    }
}