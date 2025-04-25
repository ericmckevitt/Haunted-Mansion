using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image[] hearts; // Assign Heart1, Heart2, ..., Heart5 in the Inspector
    public GameObject gameOverText; // Assign a UI Text object for "Game Over"
    
    private int lives;
    public StartMenu startMenu;

    [Header("SFX")]   
    public AudioClip hurtClip;       // drag in your damage sound
    private AudioSource sfxSource;

    void Start()
    {
        if (startMenu == null)
        {
            startMenu = FindObjectOfType<StartMenu>();
        }
        int difficulty = startMenu.GetDifficulty();

        switch (difficulty)
        {
            case 0: lives = 5; break; // Easy
            case 1: lives = 4; break; // Medium
            case 2: lives = 3; break; // Hard
            default: lives = 5; break;
        }

        // Ensure each lamp has its own AudioSource for 3D spatial SFX
        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake  = false;
        sfxSource.loop         = false;
        sfxSource.spatialBlend = 1f;       // fully 3D
        sfxSource.minDistance  = 1f;       // falloff starts at 1 unit
        sfxSource.maxDistance  = 5f;      // cut off by 15 units

        // Enable only relevant hearts based on difficulty
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < lives;
        }

        gameOverText.SetActive(false); // Hide Game Over text at the start
    }

    public void TakeDamage()
    {
        // ── play “ouch” sound ──
        if (sfxSource != null && hurtClip != null)
            sfxSource.PlayOneShot(hurtClip, 3f);

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

    public void GameOver()
    {
        lives = 0; //make them die :)
        gameOverText.SetActive(true); // Show the Game Over text
        // Optionally, you can stop player movement or restart the level here
        InputManager.DisableInput();
        Time.timeScale = 0f; // freeze game too, no noise plz
    }

    public void SetLives() {
        int difficulty = startMenu.GetDifficulty();

        switch (difficulty)
        {
            case 0: lives = 5; break; // Easy
            case 1: lives = 4; break; // Medium
            case 2: lives = 3; break; // Hard
            default: lives = 5; break;
        }

        // Enable only relevant hearts based on difficulty
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < lives;
        } 
    }
}
