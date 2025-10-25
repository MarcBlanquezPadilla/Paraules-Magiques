using UnityEngine;

public class MussolBehaviour : MonoBehaviour
{
    Animator _animator;
    [SerializeField] GameObject text;
    bool showing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        showing = false;
        text.SetActive(false);
    }

    public void ShowPista()
    {
        if (!showing)
        {
            _animator.SetInteger("state", 1);
            text.SetActive(true);
            showing = true;
        }
        
    }

    public void HidePista()
    {
        if (showing)
        {
            _animator.SetInteger("state", 0);
            text.SetActive(false);
            showing = false;
        }
        
    }
}
