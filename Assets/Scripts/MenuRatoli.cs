using TMPro;
using UnityEngine;

public class MenuRatoli : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    
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

    public void ChangeScene(string s)
    {
        ScenesManager.Instance.ChangeScene(s);
    }
}
