using UnityEngine;
using UnityEngine.UI; // ¡Necesario para usar el CanvasScaler!

// Esto fuerza a que el script deba estar en un objeto que tenga un CanvasScaler
[RequireComponent(typeof(CanvasScaler))]
public class AdaptiveCanvasScaler : MonoBehaviour
{
    // Guardamos la referencia al CanvasScaler
    private CanvasScaler canvasScaler;

    // Calculamos la relación de aspecto 9:16 una sola vez
    // (Usamos 9.0f para forzar que sea una división con decimales)
    private const float targetAspect = 9.0f / 16.0f; // Esto es 0.5625

    // Variables para guardar la resolución anterior y optimizar
    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        AdjustCanvasMatch();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            AdjustCanvasMatch();

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    private void AdjustCanvasMatch()
    {
        float currentAspect = (float)Screen.width / (float)Screen.height;

        if (currentAspect > targetAspect)
        {
            canvasScaler.matchWidthOrHeight = 1f;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0f;
        }
    }
}