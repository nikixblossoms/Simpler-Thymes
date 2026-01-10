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
        // Slot already has an item → ignore drop
        if (transform.childCount > 0)
            return;

        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        DraggableItem item = dropped.GetComponent<DraggableItem>();
        if (item == null) return;

        // Handle Give Customer slot
        if (isGiveCustomerSlot)
        {
            kitchenManager.GiveToCustomer();
        }
        else
        {
            // Normal step slot
            if (!kitchenManager.CanDoStep(stepForThisSlot))
                return;

            kitchenManager.DoStep(stepForThisSlot);
        }

        // Move the item into this slot
        item.parentAfterDrag = transform;
        item.transform.SetParent(transform);
        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
