using UnityEngine;

public class PossessedObject : MonoBehaviour
{
    public Transform player;     // Reference to the player
    public float moveSpeed = 2f; // Movement speed of the possessed object
    public float detectionRange = 30f; // Range within which the object can move
    public AudioClip scapeSound;
    public StartMenu gameState;

    private bool canMove = true;
    private AudioSource audioSource;

    void Start()
    {
        // Get reference to audio source on this object
        audioSource = GetComponent<AudioSource>();
        if (gameState == null)
        {
            gameState = FindObjectOfType<StartMenu>();
        }
    }

    public void StartPosession(Transform newPlayer) {
        player = newPlayer;
        enabled = true;

        // Ensure we have a working AudioSource
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null && scapeSound != null)
        {
            audioSource.clip = scapeSound;
            // Only play if the game has started
            if (gameState != null && gameState.gameStarted && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        Debug.Log("Possessed object: " + gameObject.name);
        Update(); //seems to catch bug where it dosent start moving again right away
    }

    public void StopPosession()
    {
        enabled = false; 

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // Only move/play audio if the game has started
        if (gameState == null || !gameState.gameStarted)
        {
            // If the game hasn't started, stop the audio and do nothing else
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();

            return;
        }

        // Check distance
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectionRange) return;

        if (CanPlayerSeeMe())
        {
            canMove = false; // Freeze when visible
        }
        else
        {
            canMove = true;  // Move when not visible
        }

        if (canMove && enabled)
        {
            MoveTowardPlayer();

            // Normalize dist so that:
            //   dist <= minDistance => 0
            //   dist >= maxDistance => 1
            Debug.Log(dist);
            float normalized = Mathf.InverseLerp(31f, 300f, dist);

            // Volume is 1 when close, 0 when far
            float volume = 1f - Mathf.Pow(normalized, 2f); 
            volume = Mathf.Clamp01(volume);

            audioSource.volume = volume;

            if (audioSource != null && !audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Pause();
        }
    }

    bool CanPlayerSeeMe()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
        {
            if (hit.transform != player)
            {
                return false; // There's an obstacle in the way
            }
        }

        // Dot product check for player's facing direction
        float dotProduct = Vector3.Dot(player.forward, -directionToPlayer);
        return dotProduct > 0.5f; // Fine-tune this threshold
    }

    // Detect collision with player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ensure the "Player" tag
        {
            StopPosession();
            Debug.Log("Possessed object hit the player!");

            other.GetComponent<PlayerHealth>().TakeDamage(); // Call TakeDamage function in player health to lose a heart
            other.GetComponent<PlayerScore>().TakeDamage(); // Call TakeDamage function in player score to lose points
        }

    }

    void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
