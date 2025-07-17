using UnityEngine;

public class InfiniteRunnerManager : MonoBehaviour
{
    public static InfiniteRunnerManager Instance { get; private set; }

    public float scrollSpeed;
    public GameObject obstacleSpawnPoint;
    public GameObject obstacleDespawnPoint;
    public InfiniteRunnerObstacle[] obstaclePrefabs;
    public float spawnRate;

    float lastSpawnTime;

    void Awake()
    {
        Instance = this;
        lastSpawnTime = Time.time;
    }

    void Update()
    {
        // TODO: increase over time //
        var spawnTime = spawnRate;

        if (Time.time - lastSpawnTime > spawnTime)
        {
            var choice = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            var obstacle = GameObject.Instantiate(choice, transform);
            obstacle.transform.position = obstacleSpawnPoint.transform.position;

            lastSpawnTime = Time.time;
        }
    }
}
