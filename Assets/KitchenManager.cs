using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KitchenManager : MonoBehaviour
{
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

    [System.Serializable]
    public class RecipeData
    {
        public string recipeName;
        public int thymeRequired;
        public RecipeStep[] requiredSteps;
    }

    [Header("References")]
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

    private RecipeData currentRecipe;
    private bool[] completedSteps;

    void Start()
    {
        if (!GameManager.Instance.hasActiveOrder)
            SpawnCustomer();
        else
            LoadExistingOrder();
    }

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

    public void DoStep(RecipeStep step)
    {
        // Check thyme for Chop/Chop2
        if ((step == RecipeStep.Chop || step == RecipeStep.Chop2) &&
            GameManager.Instance.thymeCount < currentRecipe.thymeRequired)
        {
            ShowWarning("You do not have enough thyme!");
            return;
        }

        // Process the step
        for (int i = 0; i < currentRecipe.requiredSteps.Length; i++)
        {
            if (currentRecipe.requiredSteps[i] == step && !completedSteps[i])
            {
                completedSteps[i] = true;
                GameManager.Instance.completedSteps = completedSteps; // persist progress
                UpdateOrderUI();
                break;
            }
        }

        // Check if all steps are done after this action
        CheckRecipeComplete();
    }


    // Returns true if all steps completed
    bool CheckRecipeComplete()
    {
        for (int i = 0; i < completedSteps.Length; i++)
        {
            if (!completedSteps[i])
                return false; // not complete
        }

        Debug.Log("All recipe steps done. Ready to serve!");
        return true;
    }

    // Called by the Give Customer button
    public void GiveToCustomer()
    {
        // Use CheckRecipeComplete to decide
        if (!CheckRecipeComplete())
        {
            FailOrder("Customer is angry! Order incomplete.");
            return;
        }

        // All steps complete → finish order
        FinishRecipe();
    }


    void FinishRecipe()
    {
        GameManager.Instance.thymeCount -= currentRecipe.thymeRequired;
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

    void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            CancelInvoke(nameof(ClearWarning));
            Invoke(nameof(ClearWarning), 2f);
        }
    }

    void ClearWarning()
    {
        if (warningText != null)
            warningText.text = "";
    }

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

    void UpdateStepButtons()
    {
        if (chopButton != null) chopButton.interactable = RecipeUsesStep(RecipeStep.Chop);
        if (grabPotButton != null) grabPotButton.interactable = RecipeUsesStep(RecipeStep.GrabPot);
        if (pourWaterInPotButton != null) pourWaterInPotButton.interactable = RecipeUsesStep(RecipeStep.PourWaterInPot);
        if (placeThymeInCupButton != null) placeThymeInCupButton.interactable = RecipeUsesStep(RecipeStep.PlaceThymeInCup);
        if (pourWaterInCupButton != null) pourWaterInCupButton.interactable = RecipeUsesStep(RecipeStep.PourWaterInCup);
        if (chop2Button != null) chop2Button.interactable = RecipeUsesStep(RecipeStep.Chop2);
        if (grabDoughButton != null) grabDoughButton.interactable = RecipeUsesStep(RecipeStep.GrabDough);
        if (placeDoughInBowlButton != null) placeDoughInBowlButton.interactable = RecipeUsesStep(RecipeStep.PlaceDoughInBowl);
        if (placeThymeInBowlButton != null) placeThymeInBowlButton.interactable = RecipeUsesStep(RecipeStep.PlaceThymeInBowl);
        if (kneadButton != null) kneadButton.interactable = RecipeUsesStep(RecipeStep.Knead);
        if (formDoughButton != null) formDoughButton.interactable = RecipeUsesStep(RecipeStep.FormDough);
        if (bakeButton != null) bakeButton.interactable = RecipeUsesStep(RecipeStep.Bake);
        if (giveCustomerButton != null) giveCustomerButton.interactable = true; // always interactable
    }

    bool RecipeUsesStep(RecipeStep step)
    {
        foreach (RecipeStep s in currentRecipe.requiredSteps)
            if (s == step) return true;
        return false;
    }

    #region Button Wrappers
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
    #endregion
}
