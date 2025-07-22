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
    float timeOfLastSpeedup = 0;
    float timeOfStartActualSpeedup = 0;
    bool isDistracted = false;
    bool isSpeedingUp = false;

    [Header("Timing")]
    [SerializeField] float speedupSharpness = 5;
    [SerializeField] float fastSpeed = 6;
    [SerializeField] float timeToSpeedup = 5;
    [SerializeField] float timeToYell = 60 * 5;

    [Header("Professor References")]
    [SerializeField] Animator profAnimator;

    [Header("Laptop References")]
    [SerializeField] RenderTexture laptopTexture;
    [SerializeField] AppView laptopView;

    public async UniTask Yell()
    {
        isDistracted = false;
        foreach (var app in AppManager.Instance.OpenApps.ToList())
        {
            if (app.AppRoot.isDistraction)
                AppManager.Instance.CloseApp(app);
        }

        AppManager.Instance.CanOpenApps = false;

        Debug.Log("YELL");
        profAnimator.Play("profBark", 0);
        await AudioManager.PlayBarkAsync((int)SpecificBarks.GENERICBARK1);
        speedupEffect = 0;
        AudioManager.ChangeLectureSpeed(1);

        AppManager.Instance.CanOpenApps = true;
    }

    public async UniTask FinishLecture()
    {
        AudioManager.PauseLecture();
        AudioManager.PlaySound(SoundType.STARTQUIZ);

        AppManager.Instance.CanOpenApps = false;
        AppManager.Instance.OpenApp("Quiz");

        // TODO: actual async logic here //
        await UniTask.Yield();
    }

    IEnumerator Start()
    {
        // Set up render texture for laptop //
        yield return SceneManager.LoadSceneAsync("Laptop", LoadSceneMode.Additive);
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
                timeOfLastSpeedup = Time.timeSinceLevelLoad;
                isDistracted = true;
            }
        }
        else
        {
            isDistracted = false;
        }

        var shouldSpeedUp = isDistracted && Time.timeSinceLevelLoad - timeOfLastSpeedup > timeToSpeedup;
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

        speedupEffect = MathUtil.EaseTowards(
            speedupEffect,
            targetSpeedupEffect,
            speedupSharpness,
            Time.deltaTime);

        AudioManager.ChangeLectureSpeed(Mathf.Lerp(1, fastSpeed, speedupEffect));

        if (AudioManager.LectureIsFinished || Input.GetKeyDown(KeyCode.Alpha9))
        {
            FinishLecture().Forget();
        }
    }
}
