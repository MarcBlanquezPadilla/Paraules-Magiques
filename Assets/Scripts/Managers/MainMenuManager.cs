using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MenuPrincipal; 
    public GameObject MenuSecundario; 
    public GameObject MenuSettings;

    private GameObject LastMenu;

    [SerializeField] GameObject mouseCounter;
    [SerializeField] GameData data;

    private void Awake()
    {
        
    }

    private void Start()
    {
        MenuPrincipal.SetActive(true);
        MenuSettings.SetActive(false);
        MenuSecundario.SetActive(false);
        AudioManager.Instance.PlaySong("musicaMenu");
        mouseCounter.GetComponentInChildren<TextMeshProUGUI>().text = data.mouses.ToString();
    }

    public void OpenMenu(GameObject currentMenu)
    {
        LastMenu = currentMenu;

        currentMenu.SetActive(false);
        MenuSettings.SetActive(true);
    }

    public void CloseMenu() {
        MenuSettings.SetActive(false);
        LastMenu.SetActive(true);
    }

    public void ChangeScene(string s)
    {
        ScenesManager.Instance.ChangeScene(s);
    }
}
