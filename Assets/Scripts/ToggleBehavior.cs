using UnityEngine;
using UnityEngine.UI;

public class ToggleBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Sprite trueSprite;
    [SerializeField] Sprite falseSprite;

    Toggle toggle;
    Image toggleImage;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggleImage = GetComponentInChildren<Image>();
    }

    public void Toggle(bool toggle)
    {
        if (toggle) 
        { 
            toggleImage.sprite = trueSprite;
        }
        if (!toggle) 
        {  
            toggleImage.sprite = falseSprite;
        }
    }
}
