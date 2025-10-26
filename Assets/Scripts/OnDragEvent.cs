using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnDragEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rectTransform;

    [SerializeField] UnityEvent<GameObject> OnDragStart;
    [SerializeField] UnityEvent<GameObject> OnDragStay;
    [SerializeField] UnityEvent<GameObject> OnDragEnd;

    

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnDragStart.Invoke(this.gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragStay.Invoke(this.gameObject);
        rectTransform.anchoredPosition += eventData.delta / transform.root.localScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd.Invoke(this.gameObject);
    }
}
