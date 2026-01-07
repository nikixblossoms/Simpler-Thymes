using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KitchenManager : MonoBehaviour
{
    // =========================
    // STEP TYPES
    // =========================
    public enum RecipeStep
    {
        Chop,
        GrabPot,
        PourWaterInPot,
        PlaceThymeInCup,
        PourWaterInCup,
        Chop2,
        GrabDough,
        PlaceDoughInBowl,
        PlaceThymeInBowl,
        Knead,
        FormDough,
        Bake
    }

    // =========================
    // RECIPE DATA
    // =========================
    [System.Serializable]
    public class RecipeData
    {
        public string recipeName;
        public int thymeRequired;
        public RecipeStep[] requiredSteps;
    }

    // =========================
    // REFERENCES
    // =========================
    public RecipeData[] recipes;
    public TMP_Text orderText;
    public TMP_Text warningText;

    public Button chopButton;
    public Button grabPotButton;
    public Button pourWaterInPotButton;
    public Button placeThymeInCupButton;
    public Button pourWaterInCupButton;
    public Button chop2Button;
    public Button grabDoughButton;
    public Button placeDoughInBowlButton;
    public Button placeThymeInBowlButton;
    public Button kneadButton;
    public Button formDoughButton;
    public Button bakeButton;
    public Button giveCustomerButton;

    // =========================
    // STATE
    // =========================
    private RecipeData currentRecipe;
    private bool[] completedSteps;

    // =========================
    // UNITY
    // =========================
    void Start()
    {
        if (!GameManager.Instance.hasActiveOrder)
            SpawnCustomer();
        else
            LoadExistingOrder();
    }

    // =========================
    // ORDER SETUP
    // =========================
    void SpawnCustomer()
    {
        currentRecipe = recipes[Random.Range(0, recipes.Length)];
        completedSteps = new bool[currentRecipe.requiredSteps.Length];

        GameManager.Instance.currentRecipe = currentRecipe;
        GameManager.Instance.completedSteps = completedSteps;
        GameManager.Instance.hasActiveOrder = true;

        UpdateOrderUI();
        UpdateStepButtons();
    }

    void LoadExistingOrder()
    {
        currentRecipe = GameManager.Instance.currentRecipe;
        completedSteps = GameManager.Instance.completedSteps;

        UpdateOrderUI();
        UpdateStepButtons();
    }

    // =========================
    // STEP LOGIC
    // =========================
    public void DoStep(RecipeStep step)
    {
        // Only allow the CURRENT required step
        if (!IsCurrentStep(step))
            return;

        // Thyme check
        if (step == RecipeStep.Chop || step == RecipeStep.Chop2)
        {
            if (GameManager.Instance.thymeCount < currentRecipe.thymeRequired)
            {
                ShowWarning("You do not have enough thyme!");
                return;
            }
            GameManager.Instance.thymeCount -= currentRecipe.thymeRequired;
        }

        // Mark step complete (loop-based, as requested)
        for (int i = 0; i < currentRecipe.requiredSteps.Length; i++)
        {
            if (currentRecipe.requiredSteps[i] == step && !completedSteps[i])
            {
                completedSteps[i] = true;
                GameManager.Instance.completedSteps = completedSteps;
                break;
            }
        }

        UpdateOrderUI();
        UpdateStepButtons();
    }

    // =========================
    // GIVE CUSTOMER
    // =========================
    public void GiveToCustomer()
    {
        if (!CheckRecipeComplete())
        {
            FailOrder("Customer is angry! Order incomplete.");
            return;
        }

        FinishRecipe();
    }

    // =========================
    // COMPLETION CHECK
    // =========================
    bool CheckRecipeComplete()
    {
        foreach (bool done in completedSteps)
            if (!done) return false;

        return true;
    }

    // =========================
    // FINISH / FAIL
    // =========================
    void FinishRecipe()
    {
        GameManager.Instance.customersServed++;

        Debug.Log("Order completed!");
        ClearOrder();
        SpawnCustomer();
    }

    void FailOrder(string reason)
    {
        Debug.Log(reason);
        ClearOrder();
        SpawnCustomer();
    }

    void ClearOrder()
    {
        GameManager.Instance.hasActiveOrder = false;
        GameManager.Instance.currentRecipe = null;
        GameManager.Instance.completedSteps = null;
    }

    // =========================
    // ORDERED STEP HELPERS
    // =========================
    int GetCurrentStepIndex()
    {
        for (int i = 0; i < completedSteps.Length; i++)
        {
            if (!completedSteps[i])
                return i;
        }
        return -1;
    }

    bool IsCurrentStep(RecipeStep step)
    {
        int index = GetCurrentStepIndex();
        if (index == -1) return false;
        return currentRecipe.requiredSteps[index] == step;
    }

    // =========================
    // UI
    // =========================
    void UpdateOrderUI()
    {
        if (orderText == null) return;

        string stepsText = "";
        for (int i = 0; i < currentRecipe.requiredSteps.Length; i++)
        {
            string box = completedSteps[i] ? "[x] " : "[ ] ";
            stepsText += box + currentRecipe.requiredSteps[i] + "\n";
        }

        orderText.text =
            "Customer wants:\n" +
            currentRecipe.recipeName +
            "\n\nSteps:\n" +
            stepsText +
            "\nThyme Needed: " + currentRecipe.thymeRequired;
    }

    void ShowWarning(string message)
    {
        if (warningText == null) return;

        warningText.text = message;
        CancelInvoke(nameof(ClearWarning));
        Invoke(nameof(ClearWarning), 2f);
    }

    void ClearWarning()
    {
        if (warningText != null)
            warningText.text = "";
    }

    // =========================
    // BUTTON VISUALS
    // =========================
    void SetButtonState(Button button, bool enabled)
    {
        if (button == null) return;

        button.interactable = enabled;
        button.gameObject.SetActive(enabled);

        CanvasGroup cg = button.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = button.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = enabled ? 1f : 0.35f;
    }

    void UpdateStepButtons()
    {
        SetButtonState(chopButton, IsCurrentStep(RecipeStep.Chop));
        SetButtonState(grabPotButton, IsCurrentStep(RecipeStep.GrabPot));
        SetButtonState(pourWaterInPotButton, IsCurrentStep(RecipeStep.PourWaterInPot));
        SetButtonState(placeThymeInCupButton, IsCurrentStep(RecipeStep.PlaceThymeInCup));
        SetButtonState(pourWaterInCupButton, IsCurrentStep(RecipeStep.PourWaterInCup));
        SetButtonState(chop2Button, IsCurrentStep(RecipeStep.Chop2));
        SetButtonState(grabDoughButton, IsCurrentStep(RecipeStep.GrabDough));
        SetButtonState(placeDoughInBowlButton, IsCurrentStep(RecipeStep.PlaceDoughInBowl));
        SetButtonState(placeThymeInBowlButton, IsCurrentStep(RecipeStep.PlaceThymeInBowl));
        SetButtonState(kneadButton, IsCurrentStep(RecipeStep.Knead));
        SetButtonState(formDoughButton, IsCurrentStep(RecipeStep.FormDough));
        SetButtonState(bakeButton, IsCurrentStep(RecipeStep.Bake));

        // Always visible
        SetButtonState(giveCustomerButton, true);
    }

    // =========================
    // BUTTON WRAPPERS
    // =========================
    public void ChopStep() => DoStep(RecipeStep.Chop);
    public void GrabPotStep() => DoStep(RecipeStep.GrabPot);
    public void PourWaterInPotStep() => DoStep(RecipeStep.PourWaterInPot);
    public void PlaceThymeInCupStep() => DoStep(RecipeStep.PlaceThymeInCup);
    public void PourWaterInCupStep() => DoStep(RecipeStep.PourWaterInCup);
    public void Chop2Step() => DoStep(RecipeStep.Chop2);
    public void GrabDoughStep() => DoStep(RecipeStep.GrabDough);
    public void PlaceDoughInBowlStep() => DoStep(RecipeStep.PlaceDoughInBowl);
    public void PlaceThymeInBowlStep() => DoStep(RecipeStep.PlaceThymeInBowl);
    public void KneadStep() => DoStep(RecipeStep.Knead);
    public void FormDoughStep() => DoStep(RecipeStep.FormDough);
    public void BakeStep() => DoStep(RecipeStep.Bake);
    public void GiveCustomerButton() => GiveToCustomer();
}
