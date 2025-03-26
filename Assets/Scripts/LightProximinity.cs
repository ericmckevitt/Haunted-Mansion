using UnityEngine;

public class LightProximinity : MonoBehaviour
{

    private bool playerNearby = false;

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
        if (playerNearby && Input.GetKeyDown(KeyCode.L)) {
            Transform lightChild = transform.GetChild(0);
            if (!lightChild.gameObject.activeSelf) {
                lightChild.gameObject.SetActive(true);
                Debug.Log("Light turned on!");
            }
        }
    }
}
