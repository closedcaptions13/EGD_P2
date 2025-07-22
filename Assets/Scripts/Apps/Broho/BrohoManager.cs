using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BrohoManager : MonoBehaviour
{
    public static BrohoManager Instance { get; private set; }

    [SerializeField] private BoxCollider2D levelBounds;

    [Header("Asset References")]
    [SerializeField] private BrohoObstacle bulletPrefab;
    [SerializeField] private BrohoWarning circleWarningPrefab;
    [SerializeField] private BrohoWarning lineWarningPrefab;

    private BrohoPlayer player;

    [SerializeField] private TMP_Text time;
    private float currTime;
    // private Time startTime;

    public bool IsWithinLevel(Vector2 point)
    {
        var bounds = levelBounds.bounds;
        bounds.extents = new(bounds.extents.x, bounds.extents.y, 1e5f);
        return bounds.Contains(point);
    }

    void Awake()
    {
        Instance = this;
        player = FindAnyObjectByType<BrohoPlayer>();
    }

    void Update()
    {
        currTime = Time.time - TimeAtStart;
        time.text = $"Time: {currTime.ToString("F2")}";
    }

    public const float StandardTelegraphTime = 1.2f;

    public void Warning(BrohoWarning prefab, Vector2 position, float angle, float lifetime = StandardTelegraphTime)
    {
        var inst = GameObject.Instantiate(prefab, transform);
        inst.position = position;
        inst.angle = angle;
        inst.lifetime = lifetime;
    }

    public void ObstacleLinear(BrohoObstacle prefab, Vector2 position, Vector2 velocity, float acceleration = 0)
    {
        var inst = GameObject.Instantiate(prefab, transform);
        inst.position = position;
        inst.velocity = velocity;
        inst.acceleration = acceleration;
    }

    public async UniTask ExecuteRandomFliers()
    {
        await UniTask.WaitForSeconds(10);

        var startTime = Time.time;

        while (true)
        {
            var angle = UnityEngine.Random.Range(0, 360f);
            StartCoroutine(ExecuteSpearAt(
                (Vector2)player.transform.localPosition
                    + UnityEngine.Random.insideUnitCircle * 1.0f
                    - MathUtil.DegreesToVector2(angle) * 10,
                angle
            ).ToCoroutine());

            var tfac = 5 * Mathf.Pow(0.98f, Time.time - startTime) + 0.1f;
            await UniTask.WaitForSeconds(UnityEngine.Random.Range(StandardTelegraphTime * tfac, 0));
        }
    }

    public async UniTask ExecuteCornerKillers()
    {
        await UniTask.WaitForSeconds(20);

        while (true)
        {
            const float Radius = 10;
            StartCoroutine(ExecuteBurstAt(new Vector2(-1, -1) * Radius, 25, 10).ToCoroutine());
            StartCoroutine(ExecuteBurstAt(new Vector2(+1, -1) * Radius, 25, 10).ToCoroutine());
            StartCoroutine(ExecuteBurstAt(new Vector2(+1, +1) * Radius, 25, 10).ToCoroutine());
            StartCoroutine(ExecuteBurstAt(new Vector2(-1, +1) * Radius, 25, 10).ToCoroutine());

            await UniTask.WaitForSeconds(StandardTelegraphTime * 4);
        }
    }

    public async UniTask ExecutePatterns()
    {
        IsPlaying = true;

        var startTime = Time.time;
        float WaitTime()
            => Mathf.Lerp(StandardTelegraphTime * 1.5f, StandardTelegraphTime * 0.7f, 1 - Mathf.Pow(0.99f, Time.time - startTime));

        StartCoroutine(ExecuteRandomFliers().ToCoroutine());
        StartCoroutine(ExecuteCornerKillers().ToCoroutine());

        while (true)
        {
            StartCoroutine(ExecuteBurstAt(UnityEngine.Random.insideUnitCircle * 5).ToCoroutine());
            await UniTask.WaitForSeconds(WaitTime());

            var repeatSpearWall = UnityEngine.Random.value < 0.3f ? 2 : 1;
            for (int i = 0; i < repeatSpearWall; i++)
            {
                var loc = UnityEngine.Random.insideUnitCircle * 5;
                StartCoroutine(ExecuteSpearWallAt(loc, Vector2.SignedAngle(Vector2.left, loc) - 90, 1.5f, 10).ToCoroutine());
                await UniTask.WaitForSeconds(WaitTime());
            }

            if (UnityEngine.Random.value < 0.2f)
            {
                var angle = UnityEngine.Random.Range(0, 360f);
                StartCoroutine(ExecuteWaveWall(angle + 000, 3f, 0.0f, 20).ToCoroutine());
                StartCoroutine(ExecuteWaveWall(angle + 090, 3f, 0.0f, 20).ToCoroutine());
                StartCoroutine(ExecuteWaveWall(angle + 180, 3f, 1.5f, 20).ToCoroutine());
                StartCoroutine(ExecuteWaveWall(angle + 270, 3f, 1.5f, 20).ToCoroutine());

                await UniTask.WaitForSeconds(WaitTime() * 2f);
            }
        }
    }

    public async UniTask ExecuteBurstAt(Vector2 location, int radialCount = 10, float speed = 1)
    {
        Warning(circleWarningPrefab, location, 0, StandardTelegraphTime);

        for (var i = 0; i < radialCount; i++)
        {
            var angle = i * 360f / radialCount;
            Warning(lineWarningPrefab, location, angle, StandardTelegraphTime);
        }

        await UniTask.WaitForSeconds(StandardTelegraphTime);

        for (var i = 0; i < radialCount; i++)
        {
            var angle = i * 360f / radialCount;
            var vec = MathUtil.DegreesToVector2(angle);

            ObstacleLinear(bulletPrefab, location, vec * speed, 10f);
        }
    }

    public async UniTask ExecuteSpearAt(Vector2 location, float angle)
    {
        Warning(circleWarningPrefab, location, angle, StandardTelegraphTime);
        Warning(lineWarningPrefab, location, angle, StandardTelegraphTime);
        await UniTask.WaitForSeconds(StandardTelegraphTime);

        ObstacleLinear(bulletPrefab, location, MathUtil.DegreesToVector2(angle) * 40, 2);
    }

    public async UniTask ExecuteSpearWallAt(Vector2 location, float angle, float apart, int count)
    {
        for (var i = 0; i < count; i++)
        {
            StartCoroutine(ExecuteSpearAt(
                location + (count / 2f - i) * apart * MathUtil.DegreesToVector2(angle + 90),
                angle
            ).ToCoroutine());

            await UniTask.WaitForSeconds(1f / count);
        }
    }

    public async UniTask ExecuteWaveWall(float angle, float apart, float offset, int count, float distance = 10)
    {
        var direction = MathUtil.DegreesToVector2(angle);

        var loc = -direction * distance;
        var axs = direction.Rotate(Mathf.Deg2Rad * 90);

        for (var i = 0; i < count; i++)
        {
            StartCoroutine(ExecuteSpearAt(
                loc + (offset + (count / 2f - i) * apart) * axs,
                angle
            ).ToCoroutine());
        }

        await UniTask.Yield();
    }

    public void KillGameplay()
    {
        StopAllCoroutines();
        IsPlaying = false;
    }

    public bool IsPlaying { get; private set; }
    public float TimeAtStart { get; private set; }
    public float ClockTime => Time.time - TimeAtStart;

    public void BeginGameplay()
    {
        StartCoroutine(ExecutePatterns().ToCoroutine());
        TimeAtStart = Time.time;
    }

    void Start()
    {
        BeginGameplay();
    }

    public float GetCurrTime()
    {
        return currTime;
    }
}
