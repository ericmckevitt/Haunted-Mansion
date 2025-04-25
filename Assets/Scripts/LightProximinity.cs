using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class LightProximinity : MonoBehaviour
{
    private bool playerNearby     = false;
    private bool isFixingLight    = false;
    private bool isFixed          = false;
    private int  brokenChildIndex = -1;
    private int  lampIndex        = -1;

    public TextMeshProUGUI lightFixingText;
    private PlayerScore playerScore;
    private PlayerScore.Difficulty lastDifficulty;

    // Flicker timing
    public float minOffTime = 5f;
    public float maxOffTime = 25f;
    public float minOnTime  = 0.2f;
    public float maxOnTime  = 0.6f;

    [Header("SFX")]
    public AudioClip fixLightClip;     // drag in your equipment sound
    private AudioSource sfxSource;

    void Awake()
    {
        // Make sure we grab PlayerScore before any ResetLamp calls
        playerScore = FindObjectOfType<PlayerScore>();
    }

    void Start()
    {
        // Now that Awake has run, it's safe to finish setup
        lightFixingText = GameObject
            .Find("FixingLightIndicator")
            .GetComponent<TextMeshProUGUI>();
        lightFixingText.enabled = false;

        // Ensure each lamp has its own AudioSource for 3D spatial SFX
        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake  = false;
        sfxSource.loop         = false;
        sfxSource.spatialBlend = 1f;       // fully 3D
        sfxSource.minDistance  = 1f;       // falloff starts at 1 unit
        sfxSource.maxDistance  = 5f;      // cut off by 15 units

        // Cache our index in the PlayerScore arrays
        lampIndex = System.Array.IndexOf(
            playerScore.LightObjects, this.gameObject);

        // Initialize lastDifficulty and visuals
        lastDifficulty = playerScore.GetDifficulty();
        ResetLamp();
    }

    void Update()
    {
        // 1) Detect a difficulty change
        var currentDiff = playerScore.GetDifficulty();
        if (currentDiff != lastDifficulty)
        {
            Debug.Log($"Difficulty changed from {lastDifficulty} to {currentDiff}");
            lastDifficulty = currentDiff;
            ResetLamp();
            return; // skip fix-input this frame
        }

        // 2) Handle “click to fix” when player is in range
        if (playerNearby &&
            !isFixingLight &&
            !isFixed &&
            Input.GetMouseButtonDown(0))
        {
            StartCoroutine(FixLight());
        }
    }

    IEnumerator InitializeLamp()
    {
        // wait one frame so PlayerScore.LightFixedStates is up to date
        yield return null;

        if (lampIndex >= 0 && playerScore.LightFixedStates[lampIndex])
        {
            // pre-fixed by difficulty
            isFixed = true;
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
        }
        else
        {
            // set all off, pick one to flicker
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);

            // ...pick one to flicker, then start flicker loop
            brokenChildIndex = Random.Range(0, transform.childCount);
            StartCoroutine(FlickerLoop());
        }
    }

    /// <summary>
    /// Called from LightManager.ReinitializeLamps(), or on Start(), to reset and re-init this lamp.
    /// </summary>
    public void ResetLamp()
    {
        StopAllCoroutines();
        isFixed          = false;
        isFixingLight    = false;
        brokenChildIndex = -1;

        // Guard in case Awake/Start order hasn't assigned playerScore yet
        if (playerScore == null)
            playerScore = FindObjectOfType<PlayerScore>();

        // Re-sync so our next Update() diff-check only fires on *new* changes
        lastDifficulty = playerScore.GetDifficulty();

        StartCoroutine(InitializeLamp());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }

    IEnumerator FlickerLoop()
    {
        var bulb = transform.GetChild(brokenChildIndex).gameObject;
        while (!isFixed)
        {
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));
            bulb.SetActive(true);
            yield return new WaitForSeconds(Random.Range(minOnTime,  maxOnTime));
            if (!isFixed)
                bulb.SetActive(false);
        }
    }

    IEnumerator FixLight()
    {
        isFixingLight = true;
        lightFixingText.enabled = true;
        DisablePlayerInput();

        // ── PLAY 3D FIX SOUND ──
        if (fixLightClip != null)
            sfxSource.PlayOneShot(fixLightClip);

        // simulate fix time
        yield return new WaitForSeconds(4f);

        // Ensure the bulb stays lit
        var bulb = transform.GetChild(brokenChildIndex).gameObject;
        bulb.SetActive(true);

        if (lampIndex >= 0)
            playerScore.FixLight(lampIndex);

        isFixed       = true;
        isFixingLight = false;
        lightFixingText.enabled = false;
        EnablePlayerInput();
    }

    void DisablePlayerInput() => InputManager.DisableInput();
    void EnablePlayerInput()  => InputManager.EnableInput();
}
