using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    public Image image;

    [HideInInspector] public Transform parentAfterDrag;

    private Transform originalParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag started on " + gameObject.name);

        originalParent = transform.parent;
        parentAfterDrag = originalParent;

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Drag ended on " + gameObject.name);

        // Snap back OR into slot
        transform.SetParent(parentAfterDrag);
        rectTransform.anchoredPosition = Vector2.zero;

        image.raycastTarget = true;
    }
}
