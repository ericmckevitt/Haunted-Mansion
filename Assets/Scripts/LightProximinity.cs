using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class LightProximinity : MonoBehaviour
{
    private bool playerNearby = false;
    private bool isFixingLight = false;
    public InputManager instance = null;

    void Start()
    {
        instance = GetComponent<InputManager>();
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
        if (playerNearby && Input.GetKeyDown(KeyCode.L) && !isFixingLight)
        {
            StartCoroutine(FixLight());
        }
    }

    IEnumerator FixLight()
    {
        isFixingLight = true;
        DisablePlayerInput(); // You can connect this to your input system later

        yield return new WaitForSeconds(4f);

        Transform lightChild = transform.GetChild(0);
        if (!lightChild.gameObject.activeSelf)
        {
            lightChild.gameObject.SetActive(true);
            Debug.Log("Light turned on after fixing!");
        }

        EnablePlayerInput(); // Re-enable controls after delay
        isFixingLight = false;
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
