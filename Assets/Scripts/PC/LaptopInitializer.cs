using UnityEngine;

public class LaptopInitializer : MonoBehaviour
{
    [SerializeField] string[] initialAppIcons;

    void Start()
    {
        AppManager.Filesystem.Create("NOTES.txt", new TextContents { Value = "// Notes //" });

        foreach (var app in initialAppIcons)
        {
            var icon = AppManager.Desktop.CreateIcon();

            icon.kind = DesktopIcon.Kind.Application;
            icon.application = app;

            icon.displayName = app;
        }
    }
}
