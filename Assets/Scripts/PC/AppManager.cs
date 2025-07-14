using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        // TESTING //
        OpenApp("ExampleApp").Forget();
        OpenApp("ExampleApp").Forget();
    }

    [SerializeField] AppInstance appInstancePrefab;

    ConcurrentDictionary<string, AppInstance> openApps;
    int appCounter;

    const int AppCounterGridSize = 10;
    const float AppCounterGridScale = 500;

    public async UniTask<AppInstance> OpenApp(string name)
    {
        await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        var scene = SceneManager.GetSceneByName(name);
        var roots = scene.GetRootGameObjects();

        AppRoot root;

        if (roots.Length != 1)
        {
            throw new InvalidOperationException($"App scene should have one root object!");
        }
        else if (!roots[0].TryGetComponent(out root))
        {
            throw new InvalidOperationException($"App scene should have one root object and it should have an AppRoot component!");
        }

        appCounter++;
        root.transform.position += new Vector3(
            appCounter % AppCounterGridSize,
            appCounter / AppCounterGridSize
        ) * AppCounterGridScale;

        var inst = (AppInstance)GameObject.Instantiate(appInstancePrefab, scene);

        inst.transform.SetParent(transform, false);
        inst.RenderCamera = root.GetComponentInChildren<Camera>();
        inst.SetTitle(root.appName);
        inst.Scene = scene;

        for (var i = 0; i < root.canvas.transform.childCount; i++)
        {
            var element = root.canvas.transform.GetChild(i);
            element.transform.SetParent(inst.RenderViewTransform, false);
        }

        GameObject.Destroy(root.canvas.gameObject);
        GameObject.Destroy(inst.RenderCamera.GetComponent<AudioListener>());
        GameObject.Destroy(root.GetComponentInChildren<EventSystem>()?.gameObject);

        return inst;
    }

    public AppInstance GetApp(string name)
    {
        return openApps.GetValueOrDefault(name);
    }

    public void CloseApp(AppInstance inst)
    {
        inst.CleanupRenderTexure();
        GameObject.Destroy(inst.gameObject);

        SceneManager.UnloadSceneAsync(inst.Scene);
    }
}
