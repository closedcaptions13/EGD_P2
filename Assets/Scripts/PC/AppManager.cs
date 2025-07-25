using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }
    public static DesktopManager Desktop { get; private set; }
    public static VirtualFilesystem Filesystem { get; } = new();

    void Awake()
    {
        Instance = this;
        Desktop = GetComponentInChildren<DesktopManager>();

        if (Desktop == null)
        {
            Debug.LogError($"Could not locate DesktopManager in heirarchy of AppManager.");
        }
    }

    [SerializeField] AppInstance appInstancePrefab;
    [SerializeField] PopupError errorPopupPrefab;
    [SerializeField] SerializableDictionary<string, string> fileExtensionAssociations;

    readonly Dictionary<string, AppInstance> openApps = new();
    int appCounter;

    public bool CanOpenApps { get; set; } = true;
    public bool CanCloseApps { get; set; } = true;

    const int AppCounterGridSize = 10;
    const float AppCounterGridScale = 500;

    public IReadOnlyCollection<AppInstance> OpenApps => openApps.Values;

    public void ShowError(string error)
    {
        var popup = GameObject.Instantiate(errorPopupPrefab);
        popup.transform.SetParent(transform, false);

        popup.Message = error;
    }

    public void OpenApp(string name, params object[] arguments)
    {
        OpenAppAsync(name, arguments).Forget();
    }

    private bool isOpeningScene;
    public async UniTask<AppInstance> OpenAppAsync(string name, params object[] arguments)
    {
        if (openApps.ContainsKey(name))
            return GetApp(name);

        await UniTask.WaitUntil(() => !isOpeningScene);
        isOpeningScene = true;

        var asyncOp = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        var scene = default(Scene);

        void OnLoaded(Scene sc, LoadSceneMode mode)
        {
            scene = sc;
        }

        SceneManager.sceneLoaded += OnLoaded;

        await asyncOp;
        isOpeningScene = false;

        SceneManager.sceneLoaded -= OnLoaded;
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

        var inst = GameObject.Instantiate(appInstancePrefab);

        inst.AppRoot = root;
        root.AssignedInstance = inst;

        inst.transform.SetParent(transform, false);
        inst.RenderCamera = root.GetComponentInChildren<Camera>();
        inst.SetTitle(root.appName);
        inst.Scene = scene;

        var view = inst.VirtualCanvas.gameObject.GetComponent<AppView>();
        view.screenCamera = inst.RenderCamera;
        view.screenCaster = root.canvas.GetComponent<GraphicRaycaster>();
        view.screenCaster.enabled = false;
        inst.AppView = view;

        windowMappings.Add(view, inst);

        // for (var i = 0; i < root.canvas.transform.childCount; i++)
        // {
        //     var element = root.canvas.transform.GetChild(i);
        //     element.transform.SetParent(inst.RenderViewTransform, false);
        // }

        // GameObject.Destroy(root.canvas.gameObject);

        GameObject.Destroy(inst.RenderCamera.GetComponent<AudioListener>());
        GameObject.Destroy(root.GetComponentInChildren<EventSystem>().gameObject);

        root.Arguments = arguments;
        openApps.TryAdd(name, inst);

        return inst;
    }

    public AppInstance GetApp(string name)
    {
        return openApps.GetValueOrDefault(name);
    }

    public void CloseApp(AppInstance inst)
    {
        openApps.Remove(inst.Scene.name);

        inst.CleanupRenderTexure();
        GameObject.Destroy(inst.gameObject);

        SceneManager.UnloadSceneAsync(inst.Scene);
        windowMappings.Remove(inst.AppView);
    }

    public void HandleOpenFile(string filename)
    {
        if (!Filesystem.Exists(filename))
        {
            ShowError($"Cannot find file '{filename}'");
            return;
        }

        var extension = Path.GetExtension(filename);

        if (fileExtensionAssociations.TryGetValue(extension, out var program))
        {
            OpenApp(program, filename);
        }
        else
        {
            ShowError($"Unable to open a file of type {extension}");
        }
    }

    readonly List<RaycastResult> eventSystemRaycastResults = new();
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            eventSystemRaycastResults.Clear();
            EventSystem.current.RaycastAll(
                new(EventSystem.current)
                {
                    position = Input.mousePosition,
                },
                eventSystemRaycastResults
            );

            var hoveredInstance = null as AppInstance;

            if (AppView.Hit)
            {
                var result = AppView.Hit;
                var inst = windowMappings.GetValueOrDefault(result);

                if (inst)
                    hoveredInstance = inst;
            }

            foreach (var inst in openApps.Values)
                inst.AppRoot.IsSelectedApp = (inst == hoveredInstance);
        }
    }

    private readonly Dictionary<AppView, AppInstance> windowMappings = new();
}
