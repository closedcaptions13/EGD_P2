using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppInstance : MonoBehaviour
{
    [SerializeField] Button exitButton;
    [SerializeField] RawImage renderView;
    [SerializeField] TextMeshProUGUI title;

    Vector2 renderViewPreviousSize;

    public RectTransform RenderViewTransform => renderView.rectTransform;
    public RenderTexture RenderTexture { get; private set; }
    public Camera RenderCamera { get; set; }
    public Scene Scene { get; set; }

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

    readonly Vector3[] cornersArray = new Vector3[4];

    void Update()
    {
        renderView.rectTransform.GetWorldCorners(cornersArray);
        var size = (Vector2)(cornersArray[2] - cornersArray[0]);

        if (size != renderViewPreviousSize || RenderTexture == null)
        {
            CleanupRenderTexure();

            RenderTexture = new((int)size.x, (int)size.y, 32);
            RenderTexture.Create();

            RenderCamera.targetTexture = RenderTexture;
            renderView.texture = RenderTexture;
        }
    }

    public void CleanupRenderTexure()
    {
        if(RenderTexture)
            RenderTexture.Release();
    }
}
