using UnityEngine;
using Wrj;

public class HorizontalWarning : MonoBehaviour
{
    [SerializeField]
    ShowHideCanvas warningCanvas;


    private void ShowWarning(Vector2 screenDimensions)
    {
        if (screenDimensions.x > screenDimensions.y)
        {
            warningCanvas.IsVisible = true;
        }
        else
        {
            warningCanvas.IsVisible = false;
        }
    }

    void Start()
    {
        ScreenSizeNotifier.Instance.OnScreenChange += ShowWarning;
    }
}
