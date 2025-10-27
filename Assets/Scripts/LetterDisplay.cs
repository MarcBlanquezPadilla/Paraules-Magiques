using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LetterDisplay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    TextMeshProUGUI textMeshPro;
    [SerializeField] char letter;
    [SerializeField] State state;
    [SerializeField] Type type;

    RectTransform rectTransform;

    [SerializeField] UnityEvent<GameObject> OnDragStart;
    [SerializeField] UnityEvent<GameObject> OnDragStay;
    [SerializeField] UnityEvent<GameObject> OnDragEnd;

    [SerializeField] Color lockedColor;

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite incorrectSprite;

    private Image image;

    public enum Type
    {
        Mixed,
        Current,
        Pista
    }

    public enum State
    {
        Locked, 
        Dragable
    };

    private void Awake()
    {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponentInChildren<Image>();
        state = State.Dragable;
    }

    public void ChangeLetter(char _letter)
    {
        letter = _letter;
        textMeshPro.text = _letter.ToString();
        if (state == State.Locked) textMeshPro.color = lockedColor;
        else textMeshPro.color = Color.white;
        image.sprite = defaultSprite;
    }

    public void ChangeState(State _state)
    {
        state = _state;
    }

    public State GetState()
    {
        return state;
    }

    public char GetLetter()
    {
        return letter;
    }

    public Type GetLetterType()
    {
        return type;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        if (state == State.Dragable && type != Type.Pista) OnDragStart.Invoke(this.gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (state == State.Dragable && type != Type.Pista)
        {
            OnDragStay.Invoke(this.gameObject);
            rectTransform.anchoredPosition += eventData.delta / transform.root.localScale.x;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (state == State.Dragable && type != Type.Pista) OnDragEnd.Invoke(this.gameObject);
    }

    public void ShowCorrect(bool correct)
    {
        image.sprite = correct? correctSprite : incorrectSprite;
        state = State.Locked;
        textMeshPro.color = Color.white;
    }

}
