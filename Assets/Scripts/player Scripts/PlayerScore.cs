using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public TMP_Text scoreText; // Assign the UI text for score
    public GameObject unlitBulbPrefab; // Assign prefab for unlit bulb UI
    public GameObject litBulbPrefab;   // Assign prefab for lit bulb UI
    public Transform bulbContainer;    // UI parent with Horizontal Layout Group

    private int score = 0;
    private int totalLights;
    private int lightsFixed;
    private GameObject[] dynamicUnlitBulbs;
    private GameObject[] dynamicLitBulbs;
    private GameObject[] lightObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightObjects = GameObject.FindGameObjectsWithTag("Light");
        totalLights = lightObjects.Length;
        lightsFixed = 0;
        dynamicUnlitBulbs = new GameObject[totalLights];
        dynamicLitBulbs = new GameObject[totalLights];

        for (int i = 0; i < totalLights; i++)
        {
            GameObject unlit = Instantiate(unlitBulbPrefab, bulbContainer);
            GameObject lit = Instantiate(litBulbPrefab, bulbContainer);
            lit.SetActive(false);
            dynamicUnlitBulbs[i] = unlit;
            dynamicLitBulbs[i] = lit;
        }

        UpdateScoreUI();

        // Ensure all lit bulb icons are initially hidden
        foreach (GameObject bulb in dynamicLitBulbs)
        {
            bulb.SetActive(false);
        }
    }

    public void FixLight(int lightIndex)
    {
        if (lightIndex >= 0 && lightIndex < totalLights)
        {
            lightsFixed++;
            score += 10; // Increase score by 10
            UpdateScoreUI();

            // UI: Update light bulb icons
            if (lightIndex < dynamicUnlitBulbs.Length && lightIndex < dynamicLitBulbs.Length)
            {
                dynamicUnlitBulbs[lightIndex].SetActive(false);
                dynamicLitBulbs[lightIndex].SetActive(true);
            }
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

    public int GetLightsFixed()
    {
        return lightsFixed;
    }
}
