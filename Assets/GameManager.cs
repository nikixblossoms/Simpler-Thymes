using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public class ThymePlotData
{
    public PlantState state;
    public float progress; // how far along it is in growing/watering

    public ThymePlotData(PlantState state, float progress)
    {
        this.state = state;
        this.progress = progress;
    }
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Stats")]
    public int thymeCount = 0;
    public int thymeCollectedToday = 0;
    public int customersServed = 0;
    public int customerGoal = 6;

    [Header("Day State")]
    public bool dayStarted = false;


    [Header("Day Timer")]
    public float dayLength = 300f;
    private float timeRemaining;

    // =========================
    // KITCHEN ORDER STATE
    // =========================
    public KitchenManager.RecipeData currentRecipe;
    public bool[] completedSteps;
    public bool hasActiveOrder = false;

    // =========================
    // Thyme Plot Persistence
    // =========================
    [Header("Thyme Plots")]
    public Transform thymePlotsParent;

    public List<ThymePlotData> thymePlotsData = new List<ThymePlotData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [Header("Item slots data")]
    public Dictionary<string, string> slotItemMap = new Dictionary<string, string>();


    void Start()
    {
        timeRemaining = dayLength;
        thymeCollectedToday = 0;
        dayStarted = false; // paused until Play is pressed
    }


    void Update()
    {
        if (!dayStarted)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0 || customersServed >= customerGoal)
        {
            EndDay();
        }
    }

    public void StartDay()
    {
        ResetDay();          // resets stats & timer
        dayStarted = true;   // starts the timer
        SceneManager.LoadScene("KitchenScene");
    }

    void EndDay()
    {
        SceneManager.LoadScene("EndScene");
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public void ResetDay()
    {
        thymeCount = 0;
        thymeCollectedToday = 0;
        customersServed = 0;

        timeRemaining = dayLength;
        dayStarted = false;

        hasActiveOrder = false;
        currentRecipe = null;
        completedSteps = null;
        thymePlotsData.Clear();
    }


    public void CollectThyme(int amount)
    {
        thymeCount += amount;
        thymeCollectedToday += amount;
    }

    // -----------------------------
    // SAVE & LOAD Thyme Plots
    // -----------------------------
    // Save all thyme plots
    public void SaveThymePlots()
    {
        if (thymePlotsParent == null)
        {
            Debug.LogWarning("ThymePlotsParent not assigned!");
            return;
        }

        thymePlotsData.Clear();
        ThymePlot[] plots = thymePlotsParent.GetComponentsInChildren<ThymePlot>();
        foreach (var plot in plots)
        {
            thymePlotsData.Add(plot.GetData());
        }
    }

    public void LoadThymePlots()
    {
        if (thymePlotsParent == null)
        {
            Debug.LogWarning("ThymePlotsParent not assigned!");
            return;
        }

        ThymePlot[] plots = thymePlotsParent.GetComponentsInChildren<ThymePlot>();

        if (plots.Length != thymePlotsData.Count)
        {
            Debug.LogWarning("Plot count mismatch! Check if you have the same number of plots.");
            return;
        }

        for (int i = 0; i < plots.Length; i++)
        {
            plots[i].LoadData(thymePlotsData[i]);
        }
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "FarmScene")
        {
            // Automatically find the parent in the scene
            if (thymePlotsParent == null)
            {
                thymePlotsParent = GameObject.Find("ThymePlotsParent")?.transform;
                if (thymePlotsParent == null)
                {
                    Debug.LogError("Could not find ThymePlotsParent in the scene!");
                    return;
                }
            }

            // Now load saved plots
            LoadThymePlots();
        }
    }


    public bool IsItemInAnySlot(string itemID)
    {
        foreach (var kvp in slotItemMap)
        {
            if (kvp.Value == itemID)
                return true;
        }
        return false;
    }





}
