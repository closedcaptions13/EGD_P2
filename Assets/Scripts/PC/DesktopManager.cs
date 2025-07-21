using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class DesktopManager : MonoBehaviour
{
    [SerializeField] DesktopIcon iconPrefab;
    readonly List<DesktopIcon> allIcons = new();

    Rect bounds;

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

        var pos = icon.transform.position;
        pos.x = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
        pos.y = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
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
