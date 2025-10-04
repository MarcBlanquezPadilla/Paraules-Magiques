using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    
    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }


    public void Quit()
    {
        Application.Quit();
    }

}
