using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Message : MonoBehaviour
{
    public enum MessageType
    {
        Info,
        Warning,
        Urgent
    }
    [SerializeField]
    TextMeshProUGUI messageText;
    [SerializeField]
    Image panel;
    [SerializeField]
    Color infoColor = Wrj.ColorModernized.green;
    [SerializeField]
    Color warningColor = Wrj.ColorModernized.yellow;
    [SerializeField]
    Color errorColor = Wrj.ColorModernized.red;
    private CanvasGroup canvasGroup;
    private static Coroutine showCoroutine;

    private static Message _instance;
    void Awake ()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple \"Message\" components instantiated. One removed from " + gameObject.name + ". Instance already found on " + _instance.gameObject.name + "!");
            Destroy(this);
        }
    }
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
    public static void Show(string message, MessageType messageType = MessageType.Info, float duration = 2f, Wrj.Utils.MapToCurve.OnDone onDone = null)
    {
        if (_instance == null)
        {
            Debug.LogWarning("Message instance not found. Message not shown.");
            return;
        }
        if (showCoroutine != null)
        {
            _instance.StopCoroutine(showCoroutine);
        }
        showCoroutine = _instance.StartCoroutine(_instance.ShowCoroutine(message, messageType, duration, onDone));
    }
    private IEnumerator ShowCoroutine(string message, MessageType messageType, float duration, Wrj.Utils.MapToCurve.OnDone onDone = null)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        panel.color = messageType == MessageType.Info ? infoColor : messageType == MessageType.Warning ? warningColor : errorColor;
        messageText.color = GetContrastTextColor(panel.color);
        messageText.text = message;
        yield return Wrj.Utils.MapToCurve.Linear.FadeAlpha(canvasGroup, 1, 0.5f);
        yield return new WaitForSeconds(Mathf.Max(1f, duration - 1f));
        yield return Wrj.Utils.MapToCurve.Linear.FadeAlpha(canvasGroup, 0, 0.5f, onDone: onDone);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        messageText.text = "";
    }
    private Color GetContrastTextColor(Color color)
    {
        float y = 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
        return y >= 0.5f ? Color.black : Color.white;
    }
}
