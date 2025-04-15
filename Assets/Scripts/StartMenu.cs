using UnityEngine;
using UnityEngine.UI; // If you use Button/Dropdown from the built-in UI
using TMPro;          // If you use TextMeshPro elements

public class StartMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject startMenuCanvas; 
    // Drag your new 'StartMenuCanvas' from the Hierarchy to this in the Inspector

    [SerializeField] private TMP_Dropdown difficultyDropdown;
    // Or a regular Dropdown if you're not using TextMeshPro

    private bool gameStarted = false;

    private void Start()
    {
        // Freeze the game by setting timescale to 0
        Time.timeScale = 0f;

        Cursor.visible = true;

        // Make sure the canvas is active at the start
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(true);
    }

    // This will be called by the "Start Game" button
    public void OnStartGamePressed()
    {
        // Unfreeze the game
        Time.timeScale = 1f;

        Cursor.visible = false;

        // Hide the Start Menu
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(false);

        gameStarted = true;
    }

    // This will be called by your difficulty dropdown (if you have one)
    public void OnDifficultyChanged(int value)
    {
        // 'value' corresponds to the index of the dropdown option
        // Implement your own logic here for difficulty

        switch(difficultyDropdown.value)
        {
            case 0: // e.g. "Easy"
                Debug.Log("Difficulty set to Easy");
                break;
            case 1: // e.g. "Medium"
                Debug.Log("Difficulty set to Medium");
                break;
            case 2: // e.g. "Hard"
                Debug.Log("Difficulty set to Hard");
                break;
        }
    }
}