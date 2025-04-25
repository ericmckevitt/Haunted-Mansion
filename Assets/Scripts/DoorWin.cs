using UnityEngine;
using System.Collections;  // for IEnumerator

public class DoorController : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private Animator doorAnimator;          // drag Animator here
    [SerializeField] private PossessionManager possessionManager; // drag the object that has PossessionManager
    [SerializeField] private PlayerScore playerScore;        // drag the Player object (with PlayerScore)

    [Header("Config")]
    [SerializeField] private int extraEnemiesToSpawn = 2;    

    [Header("SFX - da rumble")]   
    public AudioClip rumbleClip;       
    private AudioSource sfxSource;

    [Header("References")]
    public PlayerHealth playerHealth;  

    private bool isOpen = false;

    void Start()
    {
        // Ensure door has its own AudioSource for 3D spatial SFX
        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake  = false;
        sfxSource.loop         = false;
        sfxSource.spatialBlend = 0.8f;      
        sfxSource.minDistance  = 0.2f;       
        sfxSource.maxDistance  = 45f;      
    }

    void Update()
    {
        // Already opened or missing score → nothing to do
        if (isOpen || playerScore == null) 
            return;

        // WIN CONDITION ─ all bulbs placed
        if (playerScore.lightsFixed == playerScore.totalLights) 
        {
            Debug.Log("Win state triggered");
            isOpen = true;  // ensure we only trigger once
            StartCoroutine(OpenDoorSequence());
        }
    }

    private IEnumerator OpenDoorSequence()
    {
        // Play the door animation
        doorAnimator.Play("door", 0, 0.0f);

        // Play the rumble SFX
        if (rumbleClip != null)
            sfxSource.PlayOneShot(rumbleClip);

        // Spawn last-minute attackers
        if (possessionManager != null)
            possessionManager.SpawnExtraPossessions(extraEnemiesToSpawn);

        // Wait for the rumble to finish
        if (rumbleClip != null)
            //yield return new WaitForSeconds(rumbleClip.length); -- too long
            yield return new WaitForSeconds(60f);
        else
            yield return null;

        // Freeze the door in its open pose
        if (doorAnimator != null)
            doorAnimator.enabled = false;

        // Disable this script so it never runs again
        this.enabled = false;

        // Finally, trigger Game Over
        playerHealth.GameOver();
    }
}
