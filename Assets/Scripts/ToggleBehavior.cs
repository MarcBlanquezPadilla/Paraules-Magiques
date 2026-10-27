using UnityEngine;
using UnityEngine.UI;

public class ToggleBehavior : MonoBehaviour
{

    [SerializeField] Sprite trueSprite;
    [SerializeField] Sprite falseSprite;

    Image toggleImage;

    private void Awake()
    {
        toggleImage = GetComponentInChildren<Image>();
    }

    public void ToggleSound()
    {
        AudioManager.Instance.PlayAudio("soBoto");
    }

    public void Toggle(bool t)
    {
        if (!toggleImage) toggleImage = GetComponentInChildren<Image>();

        GetComponent<Toggle>().isOn = t;
        if (t) 
        { 
            toggleImage.sprite = trueSprite;
        }
        if (!t) 
        {  
            toggleImage.sprite = falseSprite;
        }
    }
}
