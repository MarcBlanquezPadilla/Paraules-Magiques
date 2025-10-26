using TMPro;
using UnityEngine;

public class MenuRatoli : MonoBehaviour
{

    [SerializeField] GameData data;
    [SerializeField] TextMeshProUGUI text;
    
    void Awake()
    {
        data.SumMouse();
    }

    private void Start()
    {
        AudioManager.Instance.PlayAudio("soTrobarRatoli");
        switch (data.mode)
        {
            case 0:
                text.text = "FELICITATS! HAS TROBAT EL RATOLÍ.";
                text.fontSize = 80;
                break;
            
            case 1:
                text.text = "FELICITATS! HAS COMPLETAT " + data.wordsNum + " PARAULES I TROBAT " + data.mouseNum + " RATOLINS.";
                text.fontSize = 65;
                break;
        }
    }

    public void ChangeScene(string s)
    {
        ScenesManager.Instance.ChangeScene(s);
    }
}
