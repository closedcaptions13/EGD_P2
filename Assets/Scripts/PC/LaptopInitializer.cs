using System;
using UnityEngine;

public class LaptopInitializer : MonoBehaviour
{
    [SerializeField] AppInitializer[] apps;

    [Serializable]
    public class AppInitializer
    {
        public string appName;
        public string displayName;
        public Sprite iconSprite;
    }

    void Start()
    {
        AppManager.Filesystem.Create("NOTES.txt", new TextContents { Value = "// Notes //" });

        foreach (var app in apps)
        {
            var icon = AppManager.Desktop.CreateIcon();

            icon.kind = DesktopIcon.Kind.Application;
            icon.application = app.appName;
            icon.displayName = app.displayName;
            icon.displaySprite = app.iconSprite;
        }
    }
}
