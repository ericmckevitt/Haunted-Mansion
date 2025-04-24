// PlayerScore.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class PlayerScore : MonoBehaviour
{
    public TMP_Text scoreText;            
    public GameObject unlitBulbPrefab;    
    public GameObject litBulbPrefab;      
    public Transform bulbContainer;       

    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty difficulty = Difficulty.Hard;

    [Tooltip("0 = auto‐detect; otherwise max UI bulbs")]
    public int totalLights = 0;

    public int score = 0;
    public int lightsFixed = 0;

    private GameObject[] lightObjects;
    private GameObject[] dynamicUnlitBulbs;
    private GameObject[] dynamicLitBulbs;
    private bool[]    lightFixedStates;

    public bool[] LightFixedStates => lightFixedStates;
    public GameObject[] LightObjects   => lightObjects;

    // --- ADD: remember the last difficulty so we can detect changes ---
    private Difficulty lastDifficulty;

    void Awake()
    {
        lastDifficulty = difficulty;
    }

    void Start()
    {
        RebuildStartingLights();
    }

    void Update()
    {
        // if someone flips the enum in‐Inspector at runtime, rebuild
        if (difficulty != lastDifficulty)
        {
            SetDifficulty(difficulty);
            lastDifficulty = difficulty;
        }
    }

    void OnValidate()
    {
        if (bulbContainer != null && unlitBulbPrefab != null && litBulbPrefab != null)
            RebuildStartingLights();
    }

    public void SetDifficulty(Difficulty d)
    {
        difficulty = d;
        RebuildStartingLights();

        // tell every lamp to re‐init itself
        foreach (var lamp in FindObjectsOfType<LightProximinity>())
            lamp.ResetLamp();
    }

    private void RebuildStartingLights()
    {
        // 1) find all world lamps & reset counters
        lightObjects = GameObject.FindGameObjectsWithTag("Light");
        lightsFixed  = 0;
        score        = 0;

        int sceneCount = lightObjects.Length;
        if (totalLights <= 0) totalLights = sceneCount;
        else totalLights = Mathf.Min(totalLights, sceneCount);

        // 2) clear old icons
        for (int i = bulbContainer.childCount - 1; i >= 0; i--)
            DestroyImmediate(bulbContainer.GetChild(i).gameObject);

        // 3) recreate arrays & icons
        dynamicUnlitBulbs = new GameObject[totalLights];
        dynamicLitBulbs   = new GameObject[totalLights];
        lightFixedStates  = new bool   [totalLights];

        for (int i = 0; i < totalLights; i++)
        {
            lightFixedStates[i] = false;
            var unlit = Instantiate(unlitBulbPrefab, bulbContainer);
            var lit   = Instantiate(litBulbPrefab,   bulbContainer);
            lit.SetActive(false);
            dynamicUnlitBulbs[i] = unlit;
            dynamicLitBulbs[i]   = lit;
        }

        // 4) refresh score display
        UpdateScoreUI();

        // 5) pre‐fix based on difficulty
        int startLights = difficulty == Difficulty.Medium ? 1
                        : difficulty == Difficulty.Easy   ? 3
                        : 0;

        var indices = new List<int>();
        for (int i = 0; i < totalLights; i++) indices.Add(i);

        for (int j = 0; j < startLights && indices.Count > 0; j++)
        {
            int idx = indices[Random.Range(0, indices.Count)];
            indices.Remove(idx);
            FixLight(idx);
        }
    }

    public void FixLight(int lightIndex)
    {
        if (lightIndex < 0 || lightIndex >= totalLights) return;
        if (lightFixedStates[lightIndex]) return;

        lightFixedStates[lightIndex] = true;
        lightsFixed++;
        score += 10;
        UpdateScoreUI();

        dynamicUnlitBulbs[lightIndex].SetActive(false);
        dynamicLitBulbs[lightIndex].SetActive(true);
    }

    public void TakeDamage()
    {
        score -= 5;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
        Debug.Log($"Score updated: {score}");
    }

    public int GetLightsFixed() => lightsFixed;
}
