using UnityEngine;

public class TutorialMenuManager : MonoBehaviour
{
    public void ChangeScene(string s)
    {
        ScenesManager.Instance.ChangeScene(s);
    }
}
