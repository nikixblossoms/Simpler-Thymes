using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;


public class KitchenManager : MonoBehaviour
{
    // =========================
    // STEP TYPES
    // =========================
    public enum RecipeStep
    {
        Chop1,
        ChopThyme, 
        GrabPot,
        PourWaterInPot,
        PlaceThymeInCup,
        PourWaterInCup,
        GrabDough,
        PlaceDoughinBowl,
        PlaceThymeInBowl,
        RollDough,
        FormDough,
        Bake,
        LetBreadRest,
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
    public Image warningIcon;


    public Button chopButton;
    public Button chop2Button;
    public Button grabPotButton;
    public Button pourWaterInPotButton;
    public Button placeThymeInCupButton;
    public Button pourWaterInCupButton;
    public Button grabDoughButton;
    public Button placeDoughInBowlButton;
    public Button placeThymeInBowlButton;
    public Button rollDoughButton;
    public Button formDoughButton;
    public Button bakeButton;
    public Button bakedGoodButton;
    public Button giveCustomerButton;

    [Header("Customer Speech Bubbles")]
    public GameObject happyBubble;      // shows when order is correct
    public GameObject angryBubble;      // shows when order is wrong
    public GameObject recipeBubble;     // shows customer's requested recipe
    public float bubbleDelay = 0.5f;   // delay before showing speech bubbles


    // =========================
    // STATE
    // =========================
    private RecipeData currentRecipe;
    private bool[] completedSteps;

    [System.Serializable]
    public class StepSpawnData
    {
        public RecipeStep step;
        public GameObject prefab;
        public Transform spawnParent;
    }

    [Header("Step Spawning")]
    public StepSpawnData[] stepSpawnList;

    private GameObject currentSpawnedItem;



    string SplitByCaps(string input)
    {
        return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
    }
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
        // Hide all customer bubbles & recipe bubble immediately
        if (happyBubble != null) happyBubble.SetActive(false);
        if (angryBubble != null) angryBubble.SetActive(false);
        if (recipeBubble != null) recipeBubble.SetActive(false);

        // Pick a random recipe
        currentRecipe = recipes[Random.Range(0, recipes.Length)];
        completedSteps = new bool[currentRecipe.requiredSteps.Length];

        GameManager.Instance.currentRecipe = currentRecipe;
        GameManager.Instance.completedSteps = completedSteps;
        GameManager.Instance.hasActiveOrder = true;

        UpdateOrderUI();
        UpdateStepButtons();
        SpawnItemForCurrentStep(); 

        // Show recipe bubble (just the prefab) with a slight delay
        ShowRecipeBubble();
    }




    void LoadExistingOrder()
    {
        currentRecipe = GameManager.Instance.currentRecipe;
        completedSteps = GameManager.Instance.completedSteps;

        UpdateOrderUI();
        UpdateStepButtons();
        SpawnItemForCurrentStep();
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
        if (step == RecipeStep.Chop1 || step == RecipeStep.ChopThyme)
        {
            int required = currentRecipe.thymeRequired;
            int current = GameManager.Instance.thymeCount;

            if (current < required)
            {
                int missing = required - current;
                ShowWarning($"You do not have enough thyme! You need {missing} more");
                return;
            }

            GameManager.Instance.thymeCount -= required;
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
        SpawnItemForCurrentStep();
    }

    public bool CanDoStep(KitchenManager.RecipeStep step)
    {
        // Allow if step exists and not completed yet
        for (int i = 0; i < currentRecipe.requiredSteps.Length; i++)
        {
            if (!completedSteps[i] && currentRecipe.requiredSteps[i] == step)
            {
                Debug.Log($"[DEBUG] Step '{step}' is allowed (incomplete at index {i})");
                return true;
            }
        }

        Debug.Log($"[DEBUG] Step '{step}' is NOT allowed, all matching steps complete or not in recipe");
        return false;
    }




    // =========================
    // GIVE CUSTOMER
    // =========================
    public void GiveToCustomer()
    {
        if (!CheckRecipeComplete())
        {
            
            FailOrder();
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

    // Just show the recipe bubble GameObject after a delay
    void ShowRecipeBubble()
    {
        if (recipeBubble != null)
            recipeBubble.SetActive(false); // hide first

        StartCoroutine(ShowRecipeWithDelay());
    }

    IEnumerator ShowRecipeWithDelay()
    {
        yield return new WaitForSeconds(bubbleDelay);

        if (recipeBubble != null)
            recipeBubble.SetActive(true);
    }




   void FinishRecipe()
    {
        // Destroy the last spawned item before showing bubble
        if (currentSpawnedItem != null)
        {
            Destroy(currentSpawnedItem);
            currentSpawnedItem = null;
        }

        GameManager.Instance.customersServed++;
        StartCoroutine(ShowBubbleAndNextCustomer(true)); // happy bubble
    }

    void FailOrder()
    {
        // Destroy the last spawned item before showing bubble
        if (currentSpawnedItem != null)
        {
            Destroy(currentSpawnedItem);
            currentSpawnedItem = null;
        }

        StartCoroutine(ShowBubbleAndNextCustomer(false)); // angry bubble
    }


    [SerializeField] private float bubbleDisplayTime = 2f; // how long happy/angry bubble stays

    IEnumerator ShowBubbleAndNextCustomer(bool happy)
    {
        // Hide both bubbles first
        if (happyBubble != null) happyBubble.SetActive(false);
        if (angryBubble != null) angryBubble.SetActive(false);

        // Show the correct bubble
        if (happy && happyBubble != null)
            happyBubble.SetActive(true);
        else if (!happy && angryBubble != null)
            angryBubble.SetActive(true);

        // Wait for a few seconds
        yield return new WaitForSeconds(bubbleDisplayTime);

        // Hide bubble
        if (happy && happyBubble != null)
            happyBubble.SetActive(false);
        else if (!happy && angryBubble != null)
            angryBubble.SetActive(false);

        // Clear current order
        ClearOrder();

        // Spawn next customer
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

    public bool IsCurrentStep(RecipeStep step)
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
            stepsText += box + SplitByCaps(currentRecipe.requiredSteps[i].ToString()) + "\n";

        }

        

        orderText.text =
            "" +
            stepsText;
    }

    public void ShowWarning(string message)
    {
        if (warningText == null) return;

        warningText.text = message;

        if (warningIcon != null)
            warningIcon.gameObject.SetActive(true);

        CancelInvoke(nameof(ClearWarning));
        Invoke(nameof(ClearWarning), 2f);
    }

    void ClearWarning()
    {
        if (warningText != null)
            warningText.text = "";

        if (warningIcon != null)
            warningIcon.gameObject.SetActive(false);
    }


    void ShowCustomerBubble(bool happy)
    {
        // Hide both first
        if (happyBubble != null) happyBubble.SetActive(false);
        if (angryBubble != null) angryBubble.SetActive(false);

        // Show the correct bubble after a short delay
        StartCoroutine(ShowBubbleWithDelay(happy));
    }

    IEnumerator ShowBubbleWithDelay(bool happy)
    {
        yield return new WaitForSeconds(bubbleDelay);

        if (happy && happyBubble != null)
            happyBubble.SetActive(true);
        else if (!happy && angryBubble != null)
            angryBubble.SetActive(true);
    }



    void SpawnItemForCurrentStep()
    {
        int index = GetCurrentStepIndex();
        if (index == -1) return;

        RecipeStep currentStep = currentRecipe.requiredSteps[index];

        // Destroy previous source item
        if (currentSpawnedItem != null)
            Destroy(currentSpawnedItem);

        // Find matching spawn config
        foreach (var data in stepSpawnList)
        {
            if (data.step == currentStep && data.prefab != null)
            {
                // Spawn the source item
                currentSpawnedItem = Instantiate(data.prefab, data.spawnParent);

                // =========================
                // SET UP AS SOURCE ITEM
                // =========================
                DraggableItem drag = currentSpawnedItem.GetComponent<DraggableItem>();
                if (drag != null)
                {
                    drag.isSourceItem = true;
                    drag.sourcePrefab = data.prefab;
                    drag.sourceParent = data.spawnParent;
                }

                // Optional: assign step to slot if prefab has SlotScript
                SlotScript slot = currentSpawnedItem.GetComponent<SlotScript>();
                if (slot != null)
                    slot.stepForThisSlot = currentStep;

                return;
            }
        }

        Debug.LogWarning($"No spawn data found for step: {currentStep}");
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
        SetButtonState(chopButton, IsCurrentStep(RecipeStep.Chop1));
        SetButtonState(chop2Button, IsCurrentStep(RecipeStep.ChopThyme));
        SetButtonState(grabPotButton, IsCurrentStep(RecipeStep.GrabPot));
        SetButtonState(pourWaterInPotButton, IsCurrentStep(RecipeStep.PourWaterInPot));
        SetButtonState(placeThymeInCupButton, IsCurrentStep(RecipeStep.PlaceThymeInCup));
        SetButtonState(pourWaterInCupButton, IsCurrentStep(RecipeStep.PourWaterInCup));
        SetButtonState(grabDoughButton, IsCurrentStep(RecipeStep.GrabDough));
        SetButtonState(placeDoughInBowlButton, IsCurrentStep(RecipeStep.PlaceDoughinBowl));
        SetButtonState(placeThymeInBowlButton, IsCurrentStep(RecipeStep.PlaceThymeInBowl));
        SetButtonState(rollDoughButton, IsCurrentStep(RecipeStep.RollDough));
        SetButtonState(formDoughButton, IsCurrentStep(RecipeStep.FormDough));
        SetButtonState(bakeButton, IsCurrentStep(RecipeStep.Bake));
        SetButtonState(bakedGoodButton, IsCurrentStep(RecipeStep.LetBreadRest));

        // Always visible
        SetButtonState(giveCustomerButton, true);
    }

    // =========================
    // BUTTON WRAPPERS
    // =========================
    public void Chop1Step() => DoStep(RecipeStep.Chop1);
    public void ChopThymeStep() => DoStep(RecipeStep.ChopThyme); 
    public void GrabPotStep() => DoStep(RecipeStep.GrabPot);
    public void PourWaterInPotStep() => DoStep(RecipeStep.PourWaterInPot);
    public void PlaceThymeInCupStep() => DoStep(RecipeStep.PlaceThymeInCup);
    public void PourWaterInCupStep() => DoStep(RecipeStep.PourWaterInCup);
    public void GrabDoughStep() => DoStep(RecipeStep.GrabDough);
    public void PlaceDoughInBowlStep() => DoStep(RecipeStep.PlaceDoughinBowl);
    public void PlaceThymeInBowlStep() => DoStep(RecipeStep.PlaceThymeInBowl);
    public void RollDoughStep() => DoStep(RecipeStep.RollDough);
    public void FormDoughStep() => DoStep(RecipeStep.FormDough);
    public void BakeStep() => DoStep(RecipeStep.Bake);
    public void BakedGood() => DoStep(RecipeStep.LetBreadRest);
    public void GiveCustomerButton() => GiveToCustomer();
}
