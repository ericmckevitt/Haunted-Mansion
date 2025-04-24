using UnityEngine;
using UnityEngine.UI; // If you use Button/Dropdown from the built-in UI
using TMPro;
using System;          // If you use TextMeshPro elements
using System.Collections.Generic;  // ← for List<string>

public class StartMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject startMenuCanvas; 
    // Drag your new 'StartMenuCanvas' from the Hierarchy to this in the Inspector

    [SerializeField] private TMP_Dropdown difficultyDropdown;
    // Or a regular Dropdown if you're not using TextMeshPro

    public bool gameStarted = false; // changed public so i could reference srape sound off
    private PlayerHealth playerHealth;

    private void Start()
    {
        // Freeze the game by setting timescale to 0
        Time.timeScale = 0f;

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        Cursor.visible = true;

        // Make sure the canvas is active at the start
        if (startMenuCanvas != null)
            startMenuCanvas.SetActive(true);

        // ── Setup dropdown labels/order and fire the initial difficulty ──
        if (difficultyDropdown != null)
        {
            // 1) Force exactly [Easy, Medium, Hard]
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new List<string> { "Easy", "Medium", "Hard" });

            // 2) Wire up our handler
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);

            // 3) Sync to whatever PlayerScore currently has
            var ps = FindObjectOfType<PlayerScore>();
            int current = ps != null
                ? (int)ps.GetDifficulty()
                : 0;
            difficultyDropdown.value = current;

            // 4) Apply it once so lamps light up immediately
            OnDifficultyChanged(current);
        }
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
        playerHealth.SetLives();

        // tell LightManager to update world‐space lamps for this difficulty
        LightManager.Instance?.OnDifficultyChanged(value);

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

    public int GetDifficulty()
    {
        return difficultyDropdown != null ? difficultyDropdown.value : 0;
    }
}