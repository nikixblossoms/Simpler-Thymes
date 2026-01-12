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
    public string itemID;
    public string currentSlotID = "";

    [Header("Spawner")]
    public bool isSourceItem = false;          // True = infinite source
    public GameObject sourcePrefab;            // Prefab to clone
    public Transform sourceParent;              // Where source lives

    // Internal
    private bool isClone = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
            Debug.LogError("[DraggableItem] No Canvas found in parent hierarchy!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ===============================
        // SOURCE ITEM → SPAWN CLONE
        // ===============================
        if (isSourceItem)
        {
            GameObject clone = Instantiate(sourcePrefab, sourceParent);
            DraggableItem cloneItem = clone.GetComponent<DraggableItem>();

            cloneItem.isSourceItem = false;
            cloneItem.isClone = true;
            cloneItem.parentAfterDrag = transform.parent;

            // Move clone to canvas so it can be dragged
            clone.transform.SetParent(canvas.transform);
            clone.transform.SetAsLastSibling();

            // Disable raycast while dragging
            if (cloneItem.image != null)
                cloneItem.image.raycastTarget = false;

            // Tell EventSystem to drag the clone instead
            eventData.pointerDrag = clone;

            return;
        }

        // ===============================
        // NORMAL DRAG
        // ===============================
        parentAfterDrag = transform.parent;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        if (image != null)
            image.raycastTarget = false;

        // Remove from previous slot if needed
        if (!string.IsNullOrEmpty(currentSlotID))
        {
            if (GameManager.Instance.slotItemMap.ContainsKey(currentSlotID))
                GameManager.Instance.slotItemMap.Remove(currentSlotID);

            currentSlotID = "";
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || canvas == null) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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

        GameManager.Instance.slotItemMap[slotID] = itemID;
    }
}
