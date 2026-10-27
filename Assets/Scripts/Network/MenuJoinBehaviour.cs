using System.Net;
using TMPro;
using UnityEngine;

public class MenuJoinBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private GameObject valida;
    [SerializeField] private GameObject lobbyCanvas;


    public void OnIpFieldChange()
    {
        if (ipInputField == null) return;

        string textoLimpio = ipInputField.text.Trim();

        if (string.IsNullOrWhiteSpace(textoLimpio))
        {
            if (valida != null) valida.SetActive(false);
            return;
        }

        bool esValida = IPAddress.TryParse(textoLimpio, out _);

        if (valida != null)
        {
            valida.SetActive(!esValida);
        }
    }

    public void OnClickJoinButton()
    {
        string ipIngresada = "127.0.0.1";

        if (ipInputField != null && !string.IsNullOrWhiteSpace(ipInputField.text))
        {
            string textoLimpio = ipInputField.text.Trim();

            if (IPAddress.TryParse(textoLimpio, out _))
            {
                ipIngresada = textoLimpio;
            }
            else
            {
                Debug.LogWarning($"[MenuJoinBehaviour] La IP '{textoLimpio}' no tiene un formato válido. Usando 127.0.0.1 por defecto.");
            }
        }

        NetworkManager.Instance.StartClient(ipIngresada);
        gameObject.SetActive(false);
        lobbyCanvas.SetActive(true);
    }
}
