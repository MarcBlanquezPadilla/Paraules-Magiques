using UnityEngine;

public class BruixaBehaviour : MonoBehaviour
{
    private Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void BruixaSad()
    {
        _animator.SetTrigger("cry");
    }
    public void BruixaHappy()
    {
        _animator.SetTrigger("happy");
    }
}
