using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class AdaptiveCanvasScaler : MonoBehaviour
{
    private CanvasScaler canvasScaler;

    private const float targetAspect = 9.0f / 16.0f;

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