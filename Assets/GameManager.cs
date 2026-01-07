using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Optional: reset if you restart a day
    public void ResetDay()
    {
        thymeCount = 0;
        customersServed = 0;
        timeRemaining = dayLength;

        hasActiveOrder = false;
        currentRecipe = null;
        completedSteps = null;
    }

    public void CollectThyme(int amount)
    {
        thymeCount += amount;               // inventory
        thymeCollectedToday += amount;       // daily total
    }

}
