using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuNetworkBehaviour : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;


    private void Start()
    {
        if (inputField != null && NetworkManager.Instance != null)
        {
            inputField.text = NetworkManager.Instance.playerName;
        }
    }

    public void ChangeName(string name)
    {
        if (NetworkManager.Instance != null && !string.IsNullOrEmpty(name))
        {
            NetworkManager.Instance.ChangePlayerName(name);
        }
    }

    public void OnClickHostGame()
    {
        NetworkManager.Instance.StartHost();
    }
}
