using UnityEngine;

public class MenusManager : MonoBehaviour
{
    public GameObject MenuPrincipal; 
    public GameObject MenuSecundario; 
    public GameObject MenuSettings;

    private GameObject LastMenu;

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
