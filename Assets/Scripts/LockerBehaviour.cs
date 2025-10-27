using UnityEngine;

public class LockerBehaviour : MonoBehaviour
{
    [SerializeField] int numToUnlock;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DataManager.Instance.data.mouses < numToUnlock) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
}
