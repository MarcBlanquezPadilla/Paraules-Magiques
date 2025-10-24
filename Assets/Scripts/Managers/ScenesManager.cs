using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{

    public void ChangeScene(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }


    public void Quit()
    {
        Application.Quit();
    }

}
