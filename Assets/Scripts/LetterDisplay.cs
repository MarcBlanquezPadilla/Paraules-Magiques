using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LetterDisplay : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    char letter;

    private void Awake()
    {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ChangeLetter(char _letter)
    {
        letter = _letter;
        textMeshPro.text = _letter.ToString();
    }
}
