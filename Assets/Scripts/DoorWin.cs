using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField] private Animator doorAnimator;          // drag Animator here
    [SerializeField] private PossessionManager possessionManager; // drag the object that has PossessionManager
    [SerializeField] private PlayerScore playerScore;        // drag the Player object (with PlayerScore)

    [Header("Config")]
    [SerializeField] private int extraEnemiesToSpawn = 2;    // how many new threats

    private bool isOpen = false;


    void Update()
    {
        if (isOpen || playerScore == null) return;

        // WIN CONDITION ─ all bulbs placed
        if (playerScore.lightsFixed == playerScore.totalLights) 
        {
            Debug.Log("Win state triggered");
            OpenDoorSequence();
        }
    }

    private void OpenDoorSequence()
    {
        isOpen = true;
        doorAnimator.Play("door", 0, 0.0f);    

        // 3. spawn last‑minute attackers
        if (possessionManager != null)
            possessionManager.SpawnExtraPossessions(extraEnemiesToSpawn);

        // (optional) UI popup, sound, etc.
    }
}
