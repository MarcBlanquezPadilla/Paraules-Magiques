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

    [HideInInspector] public int letterIndex;

    RectTransform rectTransform;

    [SerializeField] UnityEvent<GameObject> OnDragStart;
    [SerializeField] UnityEvent<GameObject> OnDragStay;
    [SerializeField] UnityEvent<GameObject> OnDragEnd;

    [SerializeField] Color lockedColor;
    [SerializeField] Color remoteDragColor = new Color(1f, 0.5f, 0.5f, 0.7f);

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite incorrectSprite;

    private Image image;
    private Color originalImageColor;

    private bool isLockedByRemote = false;
    private bool dragCancelled = false;

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
        if (image != null) originalImageColor = image.color;
        state = State.Dragable;
    }

    public void ChangeLetter(char _letter)
    {
        letter = _letter;
        textMeshPro.text = _letter.ToString();
        if (state == State.Locked) textMeshPro.color = lockedColor;
        else textMeshPro.color = Color.white;
        if (type != Type.Pista && image != null) image.sprite = defaultSprite;
    }

    public void ChangeState(State _state)
    {
        state = _state;
    }

    public State GetState() => state;
    public char GetLetter() => letter;
    public Type GetLetterType() => type;

    #region Drag Handlers (Local & Network)

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLockedByRemote || state != State.Dragable || type == Type.Pista) return;
        dragCancelled = false;
        OnDragStart.Invoke(this.gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragCancelled || isLockedByRemote || state != State.Dragable || type == Type.Pista) return;

        OnDragStay.Invoke(this.gameObject);
        rectTransform.anchoredPosition += eventData.delta / transform.root.localScale.x;

        if (NetworkManager.Instance != null && NetworkManager.Instance.isMultiplayer)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            NetworkManager.Instance.SendLetterDrag(letterIndex, type == Type.Mixed, pos.x, pos.y);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragCancelled) { dragCancelled = false; return; }
        if (isLockedByRemote || state != State.Dragable || type == Type.Pista) return;

        OnDragEnd.Invoke(this.gameObject);

        if (NetworkManager.Instance != null && NetworkManager.Instance.isMultiplayer)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            NetworkManager.Instance.SendLetterDragEnd(letterIndex, type == Type.Mixed, pos.x, pos.y);
        }
    }

    #endregion

    #region Remote Control (Llamado desde WordsManager al recibir datos por red)

    public void SetRemoteLock(bool isLocked)
    {
        isLockedByRemote = isLocked;

        if (image != null)
        {
            image.color = isLocked ? remoteDragColor : originalImageColor;
        }
    }

    public void UpdateRemotePosition(Vector2 newPosition)
    {
        rectTransform.anchoredPosition = newPosition;
    }

    #endregion

    public void ShowCorrect(bool correct)
    {
        if (image != null) image.sprite = correct ? correctSprite : incorrectSprite;
        state = State.Locked;
        textMeshPro.color = Color.white;
    }

    public void CancelDrag() => dragCancelled = true;
}