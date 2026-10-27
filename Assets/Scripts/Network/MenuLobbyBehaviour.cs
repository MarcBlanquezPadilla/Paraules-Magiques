using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuLobbyBehaviour : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ipText;

    [SerializeField] TextMeshProUGUI localName;
    [SerializeField] TextMeshProUGUI remoteName;

    [SerializeField] GameObject playLocker;
    
    [SerializeField] GameObject joinCanvas;
    [SerializeField] GameObject multiplayerCanvas;

    private void Update()
    {
        ipText.text = NetworkUtils.GetLocalIPAddress();

        localName.text = NetworkManager.Instance.playerName + "(Tú)";
        remoteName.text = NetworkManager.Instance.remotePlayerName;

        playLocker.SetActive(!NetworkManager.Instance.AreBothPlayersConnected || !NetworkManager.Instance.isServer);

        if (NetworkManager.Instance == null || !NetworkManager.Instance.isMultiplayer)
        {
            ExitLobby();
            return;
        }
    }

    public void OnClickEnrere()
    {
        if (NetworkManager.Instance != null)
        {
            bool isHost = NetworkManager.Instance.isServer;

            if (isHost)
            {
                if (multiplayerCanvas != null) multiplayerCanvas.SetActive(true);
                if (joinCanvas != null) joinCanvas.SetActive(false);
            }
            else
            {
                if (multiplayerCanvas != null) multiplayerCanvas.SetActive(false);
                if (joinCanvas != null) joinCanvas.SetActive(true);
            }
            NetworkManager.Instance.SendText("DISCONNECT");
            NetworkManager.Instance.StopNetwork();
        }

        gameObject.SetActive(false);
    }
    
    public void OnClickJugar()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.isServer)
        {
            for (int i = 0; i < 5; i++)
            {
                NetworkManager.Instance.SendText("START");
            }
            ScenesManager.Instance.ChangeScene("SampleScene");
        }
    }
    
    public void ExitLobby()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.StopNetwork();
        }

        bool isHost = NetworkManager.Instance != null && NetworkManager.Instance.isServer;

        if (isHost)
        {
            if (multiplayerCanvas != null) multiplayerCanvas.SetActive(true);
            if (joinCanvas != null) joinCanvas.SetActive(false);
        }
        else
        {
            if (multiplayerCanvas != null) multiplayerCanvas.SetActive(false);
            if (joinCanvas != null) joinCanvas.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
