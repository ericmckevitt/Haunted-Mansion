using UnityEngine;

public class PossessedObject : MonoBehaviour
{
    public Transform player;     // Reference to the player
    public float moveSpeed = 2f; // Movement speed of the possessed object
    public float detectionRange = 15f; // Range within which the object can move

    private bool canMove = true;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > detectionRange)
            return; // Skip logic if out of range

        if (CanPlayerSeeMe())
        {
            canMove = false; // Freeze when visible
        }
        else
        {
            canMove = true;  // Move when not visible
        }

        if (canMove)
        {
            MoveTowardPlayer();
        }
    }

    bool CanPlayerSeeMe()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // 1. Raycast check for obstacles
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
        {
            if (hit.transform != player)
            {
                return false; // There's an obstacle in the way
            }
        }

        // 2. Dot product check for player's facing direction
        float dotProduct = Vector3.Dot(player.forward, -directionToPlayer);
        return dotProduct > 0.5f; // Fine-tune this threshold
    }

    void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        Debug.Log("Moving toward player!");
    }
}
