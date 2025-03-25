using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public TMP_Text scoreText; // Assign the UI text for score
    public GameObject[] missingLights; // Assign missing light objects
    public GameObject[] fixedLights; // Assign lit light objects

    private int score = 0;
    private int totalLights;
    private int lightsFixed = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalLights = missingLights.Length;
        UpdateScoreUI();

        // Ensure all fixed lights are initially hidden
        foreach (GameObject light in fixedLights)
        {
            light.SetActive(false);
        }
    }

    public void FixLight(int lightIndex)
    {
        if (lightIndex >= 0 && lightIndex < totalLights && missingLights[lightIndex].activeSelf)
        {
           // missingLightsUI[lightIndex].SetActive(false); // Hide unlit light
            fixedLights[lightIndex].SetActive(true); // Show lit light
            lightsFixed++;
            score += 10; // Increase score by 10
            UpdateScoreUI();
        }
    }

     // Call this when the player takes damage
    public void TakeDamage()
    {
        score -= 5; // Decrease score by 5
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        Debug.Log("Score updated: " + score);
        scoreText.text = "Score: " + score;
    }

}

