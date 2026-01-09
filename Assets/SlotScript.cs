using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotScript : MonoBehaviour, IDropHandler
{
    public KitchenManager.RecipeStep stepForThisSlot;
    public KitchenManager kitchenManager;

    void Awake()
    {
        // Auto-find if not assigned
        if (kitchenManager == null)
            kitchenManager = FindObjectOfType<KitchenManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (kitchenManager == null)
        {
            Debug.LogError("KitchenManager not found!");
            return;
        }

        if (transform.childCount > 0)
            return;

        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        DraggableItem item = dropped.GetComponent<DraggableItem>();
        if (item == null) return;

        // Block invalid steps OR insufficient thyme
        if (!kitchenManager.CanDoStep(stepForThisSlot))
        {
            return;
        }


        // Perform step
        kitchenManager.DoStep(stepForThisSlot);

        // Snap item into slot
        item.parentAfterDrag = transform;
    }
}
