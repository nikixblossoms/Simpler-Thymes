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

    /// <summary>
    /// When something is dropped onto this slot
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"[DROP] Slot {name} received drop");

        // Check if slot already has a child
        if (transform.childCount > 0)
        {
            Debug.Log($"[DROP FAIL] Slot '{name}' already has a child: {transform.GetChild(0).name}");
            return;
        }

        // Get the dragged object
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null)
        {
            Debug.Log("[DROP FAIL] Dropped object is null");
            return;
        }

        // Get the DraggableItem component
        DraggableItem item = dropped.GetComponent<DraggableItem>();
        if (item == null)
        {
            Debug.Log("[DROP FAIL] Dropped object has no DraggableItem component");
            return;
        }

        // Log the item info
        Debug.Log($"[DROP INFO] Dropped item: {item.name}, isSourceItem: {item.isSourceItem}, currentSlotID: {item.currentSlotID}");

        // NOTE: We no longer block source items, because the copy should be the one dragged
        // if (item.isSourceItem)
        // {
        //     Debug.Log("[DROP WARNING] Dropped a source item — ideally this should be a copy!");
        // }

        // Handle Give Customer slot
        if (isGiveCustomerSlot)
        {
            Debug.Log("[DROP] Dropped onto Give Customer slot");
            kitchenManager.GiveToCustomer();
        }
        else
        {
            // Check if the step is allowed
            bool canDoStep = kitchenManager.CanDoStep(stepForThisSlot);
            Debug.Log($"[DROP CHECK] Slot expects step: {stepForThisSlot}, CanDoStep returned: {canDoStep}");

            if (!canDoStep)
            {
                Debug.Log($"[DROP FAIL] Step {stepForThisSlot} cannot be done right now.");
                return;
            }

            Debug.Log($"[DROP SUCCESS] Step {stepForThisSlot} accepted, performing step");
            kitchenManager.DoStep(stepForThisSlot);
        }

        // Move the item into this slot
        item.parentAfterDrag = transform;
        item.transform.SetParent(transform);
        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Debug.Log($"[DROP COMPLETE] Item {item.name} now in slot {name}");
    }
}
