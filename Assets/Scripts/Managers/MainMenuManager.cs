using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MenuPrincipal; 
    public GameObject MenuSecundario; 
    public GameObject MenuSettings;

    private GameObject LastMenu;

    private void Awake()
    {
        AudioManager.Instance.PlaySong("musicaMenu");
    }

    private void Start()
    {
        MenuPrincipal.SetActive(true);
        MenuSettings.SetActive(false);
        MenuSecundario.SetActive(false);
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
}
