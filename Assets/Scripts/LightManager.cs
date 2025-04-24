using UnityEngine;
using System.Collections.Generic;
using TMPro;               // for TMP_Dropdown
using UnityEngine.UI;      // if you use the old Dropdown instead, swap these

public class LightManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Assign the TMP_Dropdown that selects difficulty")]
    [SerializeField] private TMP_Dropdown difficultyDropdown;

    public static LightManager Instance { get; private set; }

    private PlayerScore playerScore;
    private PlayerScore.Difficulty lastDifficulty;
    private readonly List<LightProximinity> lampScripts = new List<LightProximinity>();

    void Awake()
    {
        Instance = this;
        playerScore = FindObjectOfType<PlayerScore>();

        // Cache all LightProximity scripts on GameObjects tagged "Light"
        foreach (var go in GameObject.FindGameObjectsWithTag("Light"))
        {
            var prox = go.GetComponent<LightProximinity>();
            if (prox != null)
                lampScripts.Add(prox);
        }
    }

    void Start()
    {
        if (playerScore == null)
        {
            Debug.LogError("LightManager: no PlayerScore found in scene!");
            return;
        }

        // 1) Initialize lastDifficulty from PlayerScore
        lastDifficulty = playerScore.GetDifficulty();

        // 2) Setup the UI dropdown
        if (difficultyDropdown != null)
        {
            // clear any Designer-added options
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new List<string> { "Easy", "Medium", "Hard" });

            // sync dropdown to current difficulty
            difficultyDropdown.value = (int)lastDifficulty;

            // wire up our handler
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        }
        else
        {
            Debug.LogWarning("LightManager: difficultyDropdown not assigned.");
        }

        // 3) Turn off all world bulbs at start
        foreach (var prox in lampScripts)
            for (int i = 0; i < prox.transform.childCount; i++)
                prox.transform.GetChild(i).gameObject.SetActive(false);

        // 4) Kick off initial lamp state
        ReinitializeLamps();
    }

    void Update()
    {
        // In case someone changes difficulty programmatically elsewhere,
        // we still listen for changes in PlayerScore—and re-init lamps.
        var current = playerScore.GetDifficulty();
        if (current != lastDifficulty)
        {
            ReinitializeLamps();
            lastDifficulty = current;

            // also sync the UI dropdown
            if (difficultyDropdown != null)
                difficultyDropdown.SetValueWithoutNotify((int)current);
        }
    }

    /// <summary>
    /// ONLY handler for runtime difficulty changes.
    /// Hook this to your TMP_Dropdown's OnValueChanged.
    /// </summary>
    public void OnDifficultyChanged(int idx)
    {
        var newDiff = (PlayerScore.Difficulty)idx;
        playerScore.SetDifficulty(newDiff);
        ReinitializeLamps();
        lastDifficulty = newDiff;
    }

    /// <summary>
    /// Reset every lamp in the world to match PlayerScore.LightFixedStates[].
    /// </summary>
    public void ReinitializeLamps()
    {
        foreach (var lamp in lampScripts)
            lamp.ResetLamp();
    }
}
