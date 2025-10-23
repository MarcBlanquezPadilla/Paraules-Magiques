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
