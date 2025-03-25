using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image[] hearts; // Assign Heart1, Heart2, ..., Heart5 in the Inspector
    public GameObject gameOverText; // Assign a UI Text object for "Game Over"
    
    private int lives;

    void Start()
    {
        lives = hearts.Length; // Set lives to the number of heart images
        gameOverText.SetActive(false); // Hide Game Over text at the start
    }

    public void TakeDamage()
    {
        if (lives > 0)
        {
            lives--; // Reduce life count
            hearts[lives].enabled = false; // Hide the corresponding heart

            if (lives == 0)
            {
                GameOver();
            }
        }
    }

    void GameOver()
    {
        gameOverText.SetActive(true); // Show the Game Over text
        // Optionally, you can stop player movement or restart the level here
    }
}
