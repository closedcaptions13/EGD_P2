using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppRoot : MonoBehaviour
{
    private readonly static Dictionary<Scene, AppRoot> instances = new();

    public string appName;
    public Canvas canvas;
    public bool isDistraction;

    public AppInstance AssignedInstance { get; set; }
    public bool IsSelectedApp { get; set; } = true;

    public object[] Arguments { get; set; }

    void Awake()
    {
        instances.Add(gameObject.scene, this);
    }

    void OnDestroy()
    {
        instances.Remove(gameObject.scene);
    }

    public static AppRoot ForScene(Scene scene)
        => instances.GetValueOrDefault(scene) ?? throw new InvalidOperationException("Trying to get the AppRoot from a scene where there is none.");

    public static AppRoot ForObject(Component comp)
        => ForScene(comp.gameObject.scene);

    public static bool SelectedForObject(Component comp)
        => ForObject(comp).IsSelectedApp;
}
