using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class LightProximinity : MonoBehaviour
{
    private bool playerNearby = false;
    private bool isFixingLight = false;
    public InputManager instance = null;
    private PlayerScore playerScore;
    public TextMeshProUGUI lightFixingText;

    void Start()
    {
        instance = GetComponent<InputManager>();
        playerScore = GetComponent<PlayerScore>();
        lightFixingText = GameObject.Find("FixingLightIndicator").GetComponent<TextMeshProUGUI>();
        lightFixingText.enabled = false;
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
        if (playerNearby && Input.GetKeyDown(KeyCode.Return) && !isFixingLight)
        {
            StartCoroutine(FixLight());
        }
    }

    IEnumerator FixLight()
    {
        isFixingLight = true;
        lightFixingText.enabled = true;
        DisablePlayerInput(); 

        yield return new WaitForSeconds(4f);

        bool anyLightWasOff = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (!child.activeSelf)
            {
                child.SetActive(true);
                anyLightWasOff = true;
            }
        }

        if (anyLightWasOff)
        {
            PlayerScore score = FindObjectOfType<PlayerScore>();
            int currentIndex = score.GetLightsFixed();
            score.FixLight(currentIndex);
            Debug.Log("Light turned on after fixing!");
        }

        EnablePlayerInput(); // Re-enable controls after delay
        isFixingLight = false;
        lightFixingText.enabled = false;
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
