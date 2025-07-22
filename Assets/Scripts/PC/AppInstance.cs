using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppInstance : MonoBehaviour
{
    [SerializeField] Button exitButton;
    [SerializeField] RawImage renderView;
    [SerializeField] Canvas virtualCanvas;
    [SerializeField] TextMeshProUGUI title;

    Vector2 renderViewPreviousSize;

    public RectTransform RenderViewTransform => renderView.rectTransform;
    public Canvas VirtualCanvas => virtualCanvas;
    public RenderTexture RenderTexture { get; private set; }
    public Camera RenderCamera { get; set; }
    public Scene Scene { get; set; }
    public AppRoot AppRoot { get; set; }

    void Awake()
    {
        exitButton.onClick.AddListener(CloseSelf);
    }

    void CloseSelf()
    {
        AppManager.Instance.CloseApp(this);
    }

    public void SetTitle(string title)
    {
        this.title.text = title;
    }

    void Update()
    {
        var size = renderView.rectTransform.GetWorldBounds().size;

        if (size != renderViewPreviousSize || RenderTexture == null)
        {
            CleanupRenderTexure();

            RenderTexture = new(750 * (int)size.x / (int)size.y, 750, 32);
            RenderTexture.Create();

            RenderCamera.targetTexture = RenderTexture;
            renderView.texture = RenderTexture;

            renderViewPreviousSize = size;
        }
    }

    public void CleanupRenderTexure()
    {
        if(RenderTexture)
            RenderTexture.Release();
    }
}
