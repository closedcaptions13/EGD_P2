using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    float speedupEffect = 0;
    float targetSpeedupEffect = 0;
    float timeOfStartDistraction = 0;
    float timeOfEndDistraction = 0;
    float timeOfStartActualSpeedup = 0;
    bool isEndingDistraction = false;
    bool isDistracted = false;
    bool isSpeedingUp = false;

    [Header("Timing")]
    [SerializeField] float speedupSharpness = 5;
    [SerializeField] float slowdownSharpness = 5;
    [SerializeField] float fastSpeed = 6;
    [SerializeField] float timeToSpeedup = 5;
    [SerializeField] float timeDistractionHeld = 10;
    [SerializeField] float timeToYell = 60 * 5;

    [Header("Professor References")]
    [SerializeField] Animator profAnimator;

    [Header("UI References")]
    [SerializeField] GameObject distractedWarning;
    [SerializeField] GameObject bustedDisplay;

    [Header("Laptop References")]
    [SerializeField] RenderTexture laptopTexture;
    [SerializeField] AppView laptopView;

    private int timesBarked = 0;
    public int GetBarkID()
    {
        var id = timesBarked switch
        {
            0 => (int)SpecificBarks.GENERICBARK1,
            1 => (int)SpecificBarks.GENERICBARK2,
            _ => (int)SpecificBarks.GENERICBARK3
        };

        timesBarked++;

        return id;
    }

    public async UniTask Yell()
    {
        isDistracted = false;
        foreach (var app in AppManager.Instance.OpenApps.ToList())
        {
            if (app.AppRoot.isDistraction)
                AppManager.Instance.CloseApp(app);
        }

        AppManager.Instance.CanOpenApps = false;
        bustedDisplay.SetActive(true);

        profAnimator.Play("profBark", 0);
        AudioManager.ChangeLectureSpeed(1);
        await AudioManager.PlayBarkAsync(GetBarkID());
        speedupEffect = 0;
        
        profAnimator.Play("profBarkDone", 0);
        bustedDisplay.SetActive(false);

        AppManager.Instance.CanOpenApps = true;
    }

    public async UniTask FinishLecture()
    {
        AudioManager.PauseLecture();
        AudioManager.PlaySound(SoundType.STARTQUIZ);

        AppManager.Instance.CanOpenApps = false;
        AppManager.Instance.CanCloseApps = false;
        AppManager.Instance.OpenApp("Quiz");

        await UniTask.Yield();
    }

    IEnumerator Start()
    {
        // Set up render texture for laptop //
        var result = SceneManager.LoadSceneAsync("Laptop", LoadSceneMode.Additive);

        while (!result.isDone)
            yield return null;

        var scene = SceneManager.GetSceneByName("Laptop");
        var root = scene.GetRootGameObjects()[0];
        var camera = root.GetComponentInChildren<Camera>();

        GameObject.Destroy(root.GetComponentInChildren<EventSystem>().gameObject);
        GameObject.Destroy(root.GetComponentInChildren<AudioListener>());

        camera.targetTexture = laptopTexture;

        laptopView.screenCamera = camera;
        laptopView.screenCaster = root.GetComponentInChildren<GraphicRaycaster>();

        // Play audio //
        AudioManager.PlaySound(SoundType.LECTURE);
    }

    void Update()
    {
        // If the laptop has not yet been loaded, do nothing //
        if (!AppManager.Instance)
            return;

        if (AppManager.Instance.OpenApps.Any(x => x.AppRoot.isDistraction))
        {
            if (!isDistracted)
            {
                timeOfStartDistraction = Time.timeSinceLevelLoad;
                isDistracted = true;
            }

            isEndingDistraction = false;
        }
        else
        {
            if (!isEndingDistraction)
            {
                timeOfEndDistraction = Time.timeSinceLevelLoad;
                isEndingDistraction = true;
            }
        }

        if (isEndingDistraction && Time.timeSinceLevelLoad - timeOfEndDistraction > timeDistractionHeld)
        {
            isDistracted = false;
            isEndingDistraction = false;
        }

        var shouldSpeedUp = isDistracted && Time.timeSinceLevelLoad - timeOfStartDistraction > timeToSpeedup;
        targetSpeedupEffect = shouldSpeedUp ? 1 : 0;

        if (shouldSpeedUp)
        {
            if (!isSpeedingUp)
            {
                timeOfStartActualSpeedup = Time.timeSinceLevelLoad;
                isSpeedingUp = true;
            }
        }
        else
        {
            isSpeedingUp = false;
        }

        if (isSpeedingUp && Time.timeSinceLevelLoad - timeOfStartActualSpeedup > timeToYell)
        {
            // YELL //
            Yell().Forget();
        }

        distractedWarning.SetActive(isSpeedingUp && !bustedDisplay.activeInHierarchy);

        speedupEffect = MathUtil.EaseTowards(
            speedupEffect,
            targetSpeedupEffect,
            speedupEffect < targetSpeedupEffect ? speedupSharpness : slowdownSharpness,
            Time.deltaTime);

        AudioManager.ChangeLectureSpeed(Mathf.Lerp(1, fastSpeed, speedupEffect));

        var debugSkipLecture = Input.GetKeyDown(KeyCode.Alpha9) && Input.GetKeyDown(KeyCode.LeftAlt);
        if (AudioManager.LectureIsFinished || debugSkipLecture)
        {
            FinishLecture().Forget();
        }
    }
}
