using UnityEngine;
using UnityEngine.EventSystems;

public class SlotScript : MonoBehaviour, IDropHandler
{
    [Header("Slot Settings")]
    public bool isGiveCustomerSlot;
    public KitchenManager.RecipeStep stepForThisSlot;

    [Header("References")]
    public KitchenManager kitchenManager;


    void Awake()
    {
        if (kitchenManager == null)
            Debug.LogError("KitchenManager not assigned in Inspector!");
    }


    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        DraggableItem item = dropped.GetComponent<DraggableItem>();
        if (item == null) return;

        if (isGiveCustomerSlot)
        {
            kitchenManager.GiveToCustomer();
            return;
        }

        // ORDERED STEP CHECK
        if (!kitchenManager.IsCurrentStep(stepForThisSlot))
        {
            return;
        }


        kitchenManager.DoStep(stepForThisSlot);

    
        Destroy(item.gameObject);

    }



}
