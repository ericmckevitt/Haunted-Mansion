using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 3.0f;

    private bool isDoorOpen = false;

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            if (Input.GetMouseButtonDown(0)) // updated to left click on mouse
            {
                if (isDoorOpen)
                {
                    myDoor.Play("DoorClose", 0, 0.0f);
                    isDoorOpen = false;
                }
                else
                {
                    myDoor.Play("door", 0, 0.0f);
                    isDoorOpen = true;
                }
            }
        }
    }
}