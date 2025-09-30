using UnityEngine;
using TMPro;

public class LetterDIsplay : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    char letter;

    private void Awake()
    {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        ChangeLetter(' ');
    }

    void ChangeLetter(char _letter)
    {
        letter = _letter;
        textMeshPro.text = _letter.ToString();
    }
}
