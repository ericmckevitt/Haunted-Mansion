using UnityEngine;

public class PossessedObject : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform player;               // Who we're chasing
    public float moveSpeed = 2f;           // How fast we move
    public float detectionRange = 40f;     // Only active within this range

    [Header("Audio")]
    public AudioClip scapeSound;
    public StartMenu gameState;            // To know if the game has started

    [Header("Visibility & Movement")]
    public float unseenThreshold = 0.5f;   // Seconds unseen before we move

    private AudioSource audioSource;
    private bool canMove = false;
    private bool justPossessed = false;    // For the one-frame bypass
    private float timeSinceLastSeen = 0f;

    void Awake()
    {
        // **CRUCIAL** ensure we grab the StartMenu on creation
        gameState = FindObjectOfType<StartMenu>();
        if (gameState == null)
            Debug.LogWarning($"{name}: Awake() couldn’t find a StartMenu!");
    }

    void Start()
    {
        // Cache our AudioSource (Awake only handled gameState)
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called by PossessionManager when this object becomes possessed.
    /// </summary>
    public void StartPosession(Transform newPlayer)
    {
        // In case Awake didn’t run (or was skipped), double-check here too
        if (gameState == null)
            gameState = FindObjectOfType<StartMenu>();

        player   = newPlayer;
        enabled  = true;
        justPossessed     = true;
        timeSinceLastSeen = unseenThreshold;
        canMove           = false;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip         = scapeSound;
        audioSource.loop         = true;
        audioSource.playOnAwake  = false;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance  = 2f;
        audioSource.maxDistance  = detectionRange;

        if (gameState != null && gameState.gameStarted)
            audioSource.Play();

        // Give one immediate step without waiting for the next Update frame
        ForceChaseStep();

        Debug.Log($"Possession started on: {gameObject.name}");
    }

    private void ForceChaseStep()
    {
        if (player == null) return;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        if (audioSource != null && !audioSource.isPlaying && gameState.gameStarted)
            audioSource.Play();
    }

    public void StopPosession()
    {
        enabled = false;
        justPossessed = false;
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    void Update()
    {
        // **Re-grab** gameState if somehow lost
        if (gameState == null)
        {
            gameState = FindObjectOfType<StartMenu>();
            if (gameState == null)
            {
                Debug.LogError($"{name}: Update() still can’t find a StartMenu—aborting.");
                return;
            }
        }

        // 1) Don’t do anything until the player hits Start
        if (!gameState.gameStarted)
        {
            Debug.Log($"Breaking out update early :/ gameStarted = false");
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
            return;
        }

        // 2) First‐frame bypass
        if (justPossessed)
        {
            justPossessed = false;
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            if (audioSource != null && !audioSource.isPlaying)
                audioSource.Play();
            Debug.Log("past first frame check");
            return;
        }

        // 3) Normal chase logic
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectionRange)
        {
            Debug.Log("Dist > detectionRange, returning - prob fel thru floor");
            if (audioSource != null)
                audioSource.volume = 0f;
            return;
        }

        bool seen = CanPlayerSeeMe();
        if (seen)
        {
            canMove = false;
            timeSinceLastSeen = 0f;
        }
        else
        {
            timeSinceLastSeen += Time.deltaTime;
            canMove = (timeSinceLastSeen >= unseenThreshold);
        }

        if (canMove)
        {
            Debug.Log("Moving!");
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            if (audioSource != null)
            {
                float volume = 1f - Mathf.Exp(-0.2f * (detectionRange - dist));
                audioSource.volume = Mathf.Clamp01(volume);
                if (!audioSource.isPlaying) audioSource.Play();
            }
        }
        else if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    private bool CanPlayerSeeMe()
    {
        Vector3 toMe   = transform.position - player.position;
        float distance = toMe.magnitude;
        if (Vector3.Angle(player.forward, toMe) > 30f) return false;

        Vector3[] offsets = {
            Vector3.zero, Vector3.up * 0.5f,
            Vector3.down * 0.5f, Vector3.left * 0.5f,
            Vector3.right * 0.5f
        };

        foreach (var off in offsets)
        {
            Vector3 origin = player.position + off;
            if (Physics.Raycast(origin, (transform.position - origin).normalized,
                                out var hit, distance) &&
                hit.transform == transform)
                return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (other.CompareTag("Player"))
        {
            StopPosession();
            Debug.Log("Possessed object hit the player!");
            other.GetComponent<PlayerHealth>()?.TakeDamage();
            other.GetComponent<PlayerScore>()?.TakeDamage();
        }
    }
}
