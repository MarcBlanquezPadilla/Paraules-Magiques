using UnityEngine;

public class MenuRatoli : MonoBehaviour
{

    [SerializeField] GameData data;

    
    void Awake()
    {
        data.SumMouse();
    }

    public void ChangeScene(string s)
    {
        ScenesManager.Instance.ChangeScene(s);
    }
}
