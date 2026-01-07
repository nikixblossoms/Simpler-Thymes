using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int thymeCount = 0;
    public int customersServed = 0;
    public int customerGoal = 6;

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
        }
        
    }


    void Start()
    {
        timeRemaining = dayLength;
    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
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
}


    