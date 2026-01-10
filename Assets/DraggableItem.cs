using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    [Header("UI")]
    public Image image;

    [HideInInspector] public Transform parentAfterDrag;

    [Header("Item Data")]
    public string itemID;           // Unique item identifier
    public string currentSlotID = ""; // Slot ID this item currently occupies (empty = not in slot)

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("[DraggableItem] No Canvas found in parent hierarchy!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save original parent in case we need to snap back
        parentAfterDrag = transform.parent;

        // Move item to top level so it can drag over UI
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        // Disable raycast so it doesn't block UI during drag
        if (image != null)
            image.raycastTarget = false;

        // Remove from previous slot if exists
        if (!string.IsNullOrEmpty(currentSlotID))
        {
            if (GameManager.Instance.slotItemMap.ContainsKey(currentSlotID))
            {
                GameManager.Instance.slotItemMap.Remove(currentSlotID);
            }
            currentSlotID = "";
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null) return;

        // Move item based on mouse/finger
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Snap item back to its parent (slot or spawn)
        transform.SetParent(parentAfterDrag);
        rectTransform.anchoredPosition = Vector2.zero;

        if (image != null)
            image.raycastTarget = true;
    }

    /// <summary>
    /// Called when the item is placed into a slot
    /// </summary>
    public void SetSlot(string slotID, Transform slotTransform)
    {
        currentSlotID = slotID;
        parentAfterDrag = slotTransform;

        transform.SetParent(slotTransform);
        rectTransform.anchoredPosition = Vector2.zero;

        // Update slot map for persistence
        GameManager.Instance.slotItemMap[slotID] = itemID;
    }
}
