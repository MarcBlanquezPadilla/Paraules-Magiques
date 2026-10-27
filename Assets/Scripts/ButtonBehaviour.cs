using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void Update()
    {
        
    }

    void OnClick()
    {
        AudioManager.Instance.PlayAudio("soBoto");
    }
}
