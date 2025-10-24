using UnityEngine;

public class MenuRatoli : MonoBehaviour
{

    [SerializeField] GameData data;

    
    void Awake()
    {
        data.SumMouse(1);
    }

    public void ChangeScene(string s)
    {
        ScenesManager.Instance.ChangeScene(s);
    }
}
