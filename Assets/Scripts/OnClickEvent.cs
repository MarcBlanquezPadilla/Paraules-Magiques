using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnClickEvent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UnityEvent<GameObject> clickEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        clickEvent.Invoke(this.gameObject);
    }

}
