using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class DesktopManager : MonoBehaviour
{
    [SerializeField] DesktopIcon iconPrefab;
    readonly List<DesktopIcon> allIcons = new();

    Rect bounds;
    const int GridCount = 5;

    void Awake()
    {
        var rectTransform = transform as RectTransform;
        bounds = rectTransform.GetWorldBounds();

        AppManager.Filesystem.OnCreateFile.AddListener(CreateFileIcon);
        AppManager.Filesystem.OnDeleteFile.AddListener(DestroyFileIcon);
    }

    private void CreateFileIcon(string filename)
    {
        var icon = CreateIcon();

        icon.kind = DesktopIcon.Kind.File;
        icon.filename = filename;

        icon.displayName = filename;
    }

    private void DestroyFileIcon(string filename)
    {
        var icon = allIcons.First(x => x.kind is DesktopIcon.Kind.File && x.filename == filename);
        DestroyIcon(icon);
    }

    public DesktopIcon CreateIcon()
    {
        var icon = GameObject.Instantiate(iconPrefab);
        icon.transform.SetParent(transform, false);

        var xmod = allIcons.Count % GridCount;
        var ymod = allIcons.Count / GridCount;

        var pos = icon.transform.position;
        pos.x = Mathf.Lerp(bounds.xMin, bounds.xMax, (xmod + .5f) / GridCount);
        pos.y = Mathf.Lerp(bounds.yMin, bounds.yMax, (ymod + .5f) / GridCount);
        pos.z = transform.position.z;
        icon.transform.position = pos;

        allIcons.Add(icon);

        return icon;
    }

    public void DestroyIcon(DesktopIcon icon)
    {
        allIcons.Remove(icon);
        GameObject.Destroy(icon.gameObject);
    }
}
