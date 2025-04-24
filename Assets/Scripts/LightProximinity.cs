using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class LightProximinity : MonoBehaviour
{
    private bool playerNearby      = false;
    private bool isFixingLight     = false;
    private bool isFixed           = false;
    private int  brokenChildIndex  = -1;

    public InputManager instance   = null;
    private PlayerScore playerScore;
    public TextMeshProUGUI lightFixingText;

    // Flicker settings
    public float minOffTime = 1f;
    public float maxOffTime = 5f;
    public float minOnTime  = 0.1f;
    public float maxOnTime  = 0.5f;

    void Start()
    {
        instance        = GetComponent<InputManager>();
        playerScore     = FindObjectOfType<PlayerScore>();
        lightFixingText = GameObject
            .Find("FixingLightIndicator")
            .GetComponent<TextMeshProUGUI>();
        lightFixingText.enabled = false;

        // Kick off init that respects pre-fixed lamps
        StartCoroutine(InitializeLamp());
    }

    IEnumerator InitializeLamp()
    {
        // Wait one frame so PlayerScore has applied difficulty fixes
        yield return null;

        // Which lamp index am I?
        int lampIndex = System.Array.IndexOf(
            playerScore.LightObjects, this.gameObject);

        // If that index was pre-fixed, turn on permanently
        if (lampIndex >= 0 
            && lampIndex < playerScore.LightFixedStates.Length
            && playerScore.LightFixedStates[lampIndex])
        {
            isFixed = true;
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);
        }
        else
        {
            // Otherwise start all bulbs off...
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);

            // ...pick one to flicker, then start flicker loop
            brokenChildIndex = Random.Range(0, transform.childCount);
            StartCoroutine(FlickerLoop());
        }
    }

    // Called by LightManager when difficulty or UI changes
    public void ResetLamp()
    {
        StopAllCoroutines();
        isFixed          = false;
        isFixingLight    = false;
        brokenChildIndex = -1;
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

    void Update()
    {
        if (playerNearby 
            && Mouse.current.leftButton.wasPressedThisFrame 
            && !isFixingLight 
            && !isFixed)
        {
            StartCoroutine(FixLight());
        }
    }

    IEnumerator FlickerLoop()
    {
        GameObject bulb = transform.GetChild(brokenChildIndex).gameObject;
        while (!isFixed)
        {
            // off period
            yield return new WaitForSeconds(
                Random.Range(minOffTime, maxOffTime));

            // flicker on
            bulb.SetActive(true);
            yield return new WaitForSeconds(
                Random.Range(minOnTime, maxOnTime));

            // then off again
            if (!isFixed)
                bulb.SetActive(false);
        }
    }

    IEnumerator FixLight()
    {
        isFixingLight = true;
        lightFixingText.enabled = true;
        DisablePlayerInput();

        // simulate fix time
        yield return new WaitForSeconds(4f);

        // make sure that bulb stays on
        if (brokenChildIndex >= 0)
        {
            var bulb = transform.GetChild(brokenChildIndex).gameObject;
            if (!bulb.activeSelf)
                bulb.SetActive(true);

            // notify PlayerScore
            playerScore?.FixLight(brokenChildIndex);
        }

        isFixed        = true;
        isFixingLight  = false;
        lightFixingText.enabled = false;
        EnablePlayerInput();
    }

    void DisablePlayerInput()
    {
        InputManager.DisableInput();
    }

    void EnablePlayerInput()
    {
        InputManager.EnableInput();
    }
}
