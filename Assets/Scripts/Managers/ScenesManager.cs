using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : MonoBehaviour
{
    Image image;

    [SerializeField] float timeToFadeIn;
    [SerializeField] float timeToFadeOut;


    string sceneToChange;
    bool changedScene;
    bool transitioning;
    float transitionTime;

    #region instance
        public static ScenesManager Instance { get; private set; }
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        image = GetComponentInChildren<Image>();
        sceneToChange = "MainMenu";
        transitioning = false;
        transitionTime = 0;
    }

    private void Update()
    {
        if (transitioning)
        {
            transitionTime += Time.deltaTime;
            float totalTransitionTime = timeToFadeIn + timeToFadeOut;

            if (transitionTime < timeToFadeOut)
            {
                image.fillAmount = transitionTime / timeToFadeOut;
            }

            else if (transitionTime < totalTransitionTime) 
            {
                if (!changedScene)
                {
  
                    image.fillAmount = 1.0f;
                    SceneManager.LoadScene(sceneToChange);
                    changedScene = true;
                }

                float fadeInProgressTime = transitionTime - timeToFadeOut;
                image.fillAmount = 1.0f - (fadeInProgressTime / timeToFadeIn);
            }


            else
            {
                image.fillAmount = 0f; 
                transitioning = false; 
            }
        }
    }

    public void ChangeScene(string s)
    {
        sceneToChange = s;
        transitionTime = 0;
        changedScene = false;
        transitioning = true;
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
