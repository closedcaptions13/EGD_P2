using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BrohoManager : MonoBehaviour
{
    public static BrohoManager Instance { get; private set; }

    [SerializeField] private BoxCollider2D levelBounds;

    [Header("Asset References")]
    [SerializeField] private BrohoObstacle bulletPrefab;
    [SerializeField] private BrohoWarning circleWarningPrefab;
    [SerializeField] private BrohoWarning lineWarningPrefab;

    public bool IsWithinLevel(Vector2 point)
    {
        var bounds = levelBounds.bounds;
        bounds.extents = new(bounds.extents.x, bounds.extents.y, 1e5f);
        return bounds.Contains(point);
    }

    void Awake()
    {
        Instance = this;
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
        inst.kind = BrohoObstacle.Kind.Linear;
    }

    public void ObstacleWave(BrohoObstacle prefab, Vector2 position, Vector2 velocity, float waveRate, float waveAmp, float acceleration = 0)
    {
        var inst = GameObject.Instantiate(prefab, transform);
        inst.position = position;
        inst.velocity = velocity;
        inst.acceleration = acceleration;
        inst.waveRate = waveRate;
        inst.waveAmplitude = waveAmp;
        inst.kind = BrohoObstacle.Kind.Wave;
    }

    public async UniTask ExecutePatterns()
    {
        while (true)
        {
            ExecuteBurstAt(UnityEngine.Random.insideUnitCircle * 5).Forget();
            await UniTask.WaitForSeconds(1.5f);

            var loc = UnityEngine.Random.insideUnitCircle * 5;
            ExecuteSpearWallAt(loc, Vector2.SignedAngle(Vector2.left, loc) - 90, 1.5f, 10).Forget();
            await UniTask.WaitForSeconds(1.5f);
        }
    }

    public async UniTask ExecuteBurstAt(Vector2 location, int radialCount = 10)
    {
        Warning(circleWarningPrefab, location, 0, StandardTelegraphTime);
        await UniTask.WaitForSeconds(StandardTelegraphTime);

        for (var i = 0; i < radialCount; i++)
        {
            var angle = i * 360f / radialCount;
            var vec = MathUtil.DegreesToVector2(angle);

            ObstacleLinear(bulletPrefab, location, vec, 10f);
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
            ExecuteSpearAt(
                location + (count / 2f - i) * apart * MathUtil.DegreesToVector2(angle + 90),
                angle
            ).Forget();

            await UniTask.WaitForSeconds(1f / count);
        }
        
    }

    void Start()
    {
        ExecutePatterns().Forget();
    }
}
