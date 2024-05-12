using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ShowHideCanvas : MonoBehaviour
{
    [SerializeField]
    private bool visibilityOnAwake = false;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();   
        canvasGroup.alpha = visibilityOnAwake ? 1 : 0;
        canvasGroup.blocksRaycasts = visibilityOnAwake;
        canvasGroup.interactable = visibilityOnAwake;
    }
    public bool IsVisible
    {
        get
        {
            return canvasGroup.blocksRaycasts;
        }
        set
        {
            Wrj.Utils.MapToCurve.Linear.FadeAlpha(canvasGroup, value ? 1 : 0, 0.5f);
            canvasGroup.blocksRaycasts = value;
            canvasGroup.interactable = value;
        }
    }
    public void Toggle()
    {
        IsVisible = !IsVisible;
    }
}
