using TMPro;
using UnityEngine;

public class MenuRatoli : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject locker;
    
    private void Start()
    {
        AudioManager.Instance.PlayAudio("soTrobarRatoli");
        switch (DataManager.Instance.data.mode)
        {
            case 0:
                text.text = "FELICITATS! HAS TROBAT EL RATOLÍ.";
                text.fontSize = 80;
                break;
            
            case 1:
                text.text = "FELICITATS! HAS COMPLETAT " + DataManager.Instance.data.wordsNum + " PARAULES I TROBAT " + DataManager.Instance.data.mouseNum + " RATOLINS.";
                text.fontSize = 65;
                break;
        }
        DataManager.Instance.data.mouses += DataManager.Instance.data.mouseNum;
        DataManager.Instance.SaveData();
    }

    private void Update()
    {
        if (NetworkManager.Instance.isMultiplayer && NetworkManager.Instance.isServer)
            locker.SetActive(false);
        else if (NetworkManager.Instance.isMultiplayer && !NetworkManager.Instance.isServer)
            locker.SetActive(true);
        else locker.SetActive(false);
    }

    public void ChangeGameScene()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.isServer)
        {
            for (int i = 0; i < 5; i++)
            {
                NetworkManager.Instance.SendText("START");
            }
        }
        ScenesManager.Instance.ChangeScene("SampleScene");
    }
    public void ChangeMenuScene()
    {
        ScenesManager.Instance.ChangeScene("MainMenu");
    }
}
