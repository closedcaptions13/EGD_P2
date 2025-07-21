using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopIcon : MonoBehaviour, IPointerClickHandler
{
    public enum Kind
    {
        File,
        Application
    }

    [Header("Display Settings")]
    public Sprite displaySprite;
    public string displayName;

    [Header("File Settings")]
    public Kind kind;
    public string filename;
    public string application;

    private TextMeshProUGUI text;
    private SpriteRenderer sprite;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        text.text = displayName;
        if(displaySprite != null)
            sprite.sprite = displaySprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!AppManager.Instance.CanOpenApps)
            return;
            
        if (kind is Kind.File)
        {
            AppManager.Instance.HandleOpenFile(filename);
        }
        else if (kind is Kind.Application)
        {
            AppManager.Instance.OpenApp(application);
        }
        else
        {
            AppManager.Instance.ShowError($"Unknown file kind {kind}");
        }
    }
}
