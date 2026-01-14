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


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
            Debug.LogError("[DraggableItem] No Canvas found in parent hierarchy!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        if (image != null)
            image.raycastTarget = false;
            image.color = new Color(1f, 1f, 1f, 0.8f);

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
