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
            kitchenManager = FindObjectOfType<KitchenManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        DraggableItem item = dropped.GetComponent<DraggableItem>();
        if (item == null) return;

        // Spawned clone from source
        if (item.isSourceItem)
        {
            Debug.Log("[DROP WARNING] Source item should be a clone! Make sure OnBeginDrag spawns a clone.");
        }

        // Check Give Customer slot
        if (isGiveCustomerSlot)
        {
            kitchenManager.GiveToCustomer();
        }
        else
        {
            // Check if step is valid
            if (!kitchenManager.CanDoStep(stepForThisSlot))
            {
                return;
            }

            kitchenManager.DoStep(stepForThisSlot);
        }

        // Move item to slot
        item.parentAfterDrag = transform;
        item.currentSlotID = name;
        item.transform.SetParent(transform);
        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }


}
