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
    public TextMeshProUGUI winText;   // assign a full-screen or centered UI TextMeshProUGUI that says “Escape” and disable it in the Inspector   

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
        
        if (!Application.isPlaying)
        {
            // In Editor or before Play: full rebuild
            RebuildStartingLights();
        }
        else
        {
            // At runtime: only update fixed states and icons
            // 1) reset all fixed states
            for (int i = 0; i < totalLights; i++)
                lightFixedStates[i] = false;

            // 2) pick new pre-fixed lights based on difficulty
            int startLights = difficulty == Difficulty.Medium ? 1
                            : difficulty == Difficulty.Easy   ? 3
                            : 0;
            var indices = new List<int>();
            for (int i = 0; i < totalLights; i++) indices.Add(i);
            for (int j = 0; j < startLights && indices.Count > 0; j++)
            {
                int idx = indices[Random.Range(0, indices.Count)];
                indices.Remove(idx);
                lightFixedStates[idx] = true;
            }

            // 3) update only the UI icons
            UpdateLightIcons();

     
            // 4) update the fixed count & score so win condition can fire
            lightsFixed = 0;
            for (int i = 0; i < totalLights; i++)
                if (lightFixedStates[i])
                    lightsFixed++;
            score = lightsFixed * 10;
            UpdateScoreUI();

            // 5) tell all world lamps to re-init with the new fixed states
            LightManager.Instance?.ReinitializeLamps();
        }
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

        if (winText != null)
            winText.enabled = false;
    }

    public void FixLight(int lightIndex)
    {
        // if (lightIndex < 0 || lightIndex >= totalLights) return;
        // if (lightFixedStates[lightIndex]) return;

        lightFixedStates[lightIndex] = true;
        lightsFixed++;
        score += 10;
        UpdateScoreUI();

        dynamicUnlitBulbs[lightIndex].SetActive(false);
        dynamicLitBulbs[lightIndex].SetActive(true);

        Debug.Log($"lights fixed = {lightsFixed}");

        if (lightsFixed == totalLights && winText != null)
        {
            winText.enabled = true;
        }
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

    /// <summary>
    /// Toggle each unlit/lit icon to match lightFixedStates[].
    /// Call this whenever lightFixedStates changes (e.g. on difficulty change).
    /// </summary>
    public void UpdateLightIcons()
    {
        for (int i = 0; i < totalLights; i++)
        {
            bool fixedState = lightFixedStates[i];
            dynamicLitBulbs[i].SetActive(fixedState);
            dynamicUnlitBulbs[i].SetActive(!fixedState);
        }
    }

     /// <summary>
    /// Expose the current difficulty so other scripts can query it.
    /// </summary>
    public Difficulty GetDifficulty()
    {
        return difficulty;
    }
}
