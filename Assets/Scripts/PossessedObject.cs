using UnityEngine;

public class PossessedObject : MonoBehaviour
{
    public Transform player;     // Reference to the player
    public float moveSpeed = 2f; // Movement speed of the possessed object
    public float detectionRange = 15f; // Range within which the object can move

    private bool canMove = true;

    public void StartPosession(Transform newPlayer) {
        player = newPlayer;
        enabled = true;
        Debug.Log("Possessed object: " + gameObject.name);
        Update(); //seems to catch bug where it dosent start moving again right away
    }

    public void StopPosession()
    {
        enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if (Vector3.Distance(transform.position, player.position) > detectionRange) return; 

        if (CanPlayerSeeMe())
        {
            canMove = false; // Freeze when visible
        }
        else
        {
            canMove = true;  // Move when not visible
        }

        if (canMove && enabled)
        {
            MoveTowardPlayer();
        }
    }

    bool CanPlayerSeeMe()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
        {
            if (hit.transform != player)
            {
                return false; // There's an obstacle in the way
            }
        }

        // Dot product check for player's facing direction
        float dotProduct = Vector3.Dot(player.forward, -directionToPlayer);
        return dotProduct > 0.5f; // Fine-tune this threshold
    }

    // Detect collision with player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ensure the "Player" tag
        {
            Debug.Log("Possessed object hit the player!");

            other.GetComponent<PlayerHealth>().TakeDamage(); // Call TakeDamage function in player health to lose a heart
            other.GetComponent<PlayerScore>().TakeDamage(); // Call TakeDamage function in player score to lose points

            StopPosession();
        }

    }

    void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
