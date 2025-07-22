using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class InfiniteRunnerManager : MonoBehaviour
{
    public static InfiniteRunnerManager Instance { get; private set; }

    public float scrollSpeed;
    public GameObject obstacleSpawnPoint;
    public GameObject obstacleDespawnPoint;
    public InfiniteRunnerObstacle[] obstaclePrefabs;
    public float spawnRate;

    float lastSpawnTime;
    float playTime;

    [SerializeField] private TMP_Text time;

    public static bool WasPlaying { get; private set; }
    public static bool IsPlaying { get; private set; }
    public static bool JustStartedPlaying => IsPlaying && !WasPlaying;

    public void StartPlaying()
    {
        playTime = Time.time;
        lastSpawnTime = Time.time;
        IsPlaying = true;
    }

    public void StopPlaying()
    {
        IsPlaying = false;
    }

    void Start()
    {
        StartPlaying();
    }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        WasPlaying = IsPlaying;

        // TODO: increase over time //
        var spawnTime = spawnRate;

        if (IsPlaying && Time.time - lastSpawnTime > spawnTime)
        {
            var choice = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            var obstacle = GameObject.Instantiate(choice, transform);
            obstacle.transform.position = obstacleSpawnPoint.transform.position;

            lastSpawnTime = Time.time;
        }

        if (IsPlaying)
        {
            time.text = $"Time: {(Time.time - playTime):F2}";
        }
    }
}
