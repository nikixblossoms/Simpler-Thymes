using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    public Recipe[] recipes;

    private Recipe currentRecipe;
    private int currentStep = 0;

    void Start()
    {
        SpawnCustomer();
    }

    void SpawnCustomer()
    {
        currentRecipe = recipes[Random.Range(0, recipes.Length)];
        currentStep = 0;
        Debug.Log("Customer wants: " + currentRecipe.recipeName);
    }

    public void DoStep()
    {
        currentStep++;
        if (currentStep >= currentRecipe.stepsRequired)
        {
            FinishRecipe();
        }
    }

    void FinishRecipe()
    {
        if (GameManager.Instance.thymeCount < currentRecipe.thymeRequired)
        {
            FailOrder();
            return;
        }

        GameManager.Instance.thymeCount -= currentRecipe.thymeRequired;
        GameManager.Instance.customersServed++;

        SpawnCustomer();
    }

    void FailOrder()
    {
        SpawnCustomer();
    }
}
