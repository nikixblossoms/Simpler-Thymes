using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    void Start()
    {
        timeRemaining = dayLength;
        thymeCollectedToday = 0;
    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0 || customersServed >= customerGoal)
        {
            EndDay();
        }
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
        thymePlotsData.Clear();
        ThymePlot[] plots = Object.FindObjectsByType<ThymePlot>(FindObjectsSortMode.None);
        foreach (var plot in plots)
        {
            thymePlotsData.Add(plot.GetData());
        }
    }

    public void LoadThymePlots()
    {
        ThymePlot[] plots = Object.FindObjectsByType<ThymePlot>(FindObjectsSortMode.None);

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
        // Only reload thyme plots if we are in the Garden scene
        if (scene.name == "FarmScene")
        {
            LoadThymePlots();
        }
    }


}
