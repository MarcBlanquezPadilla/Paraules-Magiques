using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MenuPrincipal;
    public GameObject MenuSecundario;
    public GameObject MenuSettings;
    public GameObject MenuLore;

    private GameObject LastMenu;

    [SerializeField] GameObject mouseCounter;


    private void Start()
    {
        MenuPrincipal.SetActive(true);
        MenuSettings.SetActive(false);
        MenuSecundario.SetActive(false);
        MenuLore.SetActive(false);
        if (!AudioManager.Instance.IsPlaying("musicaMenu")) AudioManager.Instance.PlaySong("musicaMenu");
        mouseCounter.GetComponentInChildren<TextMeshProUGUI>().text = DataManager.Instance.data.mouses.ToString();
    }

    public void OpenMenu(GameObject nextMenu)
    {

        MenuSettings.SetActive(false);
        MenuPrincipal.SetActive(false);
        MenuLore.SetActive(false);
        MenuSecundario.SetActive(false);
        nextMenu.SetActive(true);

    }

    public void CloseMenu()
    {
        MenuSettings.SetActive(false);
        LastMenu.SetActive(true);
    }

    public void ChangeScene(string s)
    {
        if (s == "SampleScene" && DataManager.Instance.data.firstTime)
        {
            DataManager.Instance.data.firstTime=false;

            DataManager.Instance.SaveData();
            ScenesManager.Instance.ChangeScene("Tutorial");
        }
        else
        {
            DataManager.Instance.SaveData();
            ScenesManager.Instance.ChangeScene(s);
        }
    }

    public void PlayButton()
    {
        if (DataManager.Instance.data.firstTime) OpenMenu(MenuLore);
        else OpenMenu(MenuSecundario);
    }

    public void TutorialClick() 
    {

    }

    public void ChangeMode(int mode)
    {
        DataManager.Instance.ChangeMode(mode);
    }

    public void ChangeDiff(int diff)
    {
        DataManager.Instance.ChangeDifficulty(diff);
    }
}
