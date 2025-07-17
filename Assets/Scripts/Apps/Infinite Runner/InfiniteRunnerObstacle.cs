using UnityEngine;

public class InfiniteRunnerObstacle : MonoBehaviour
{
    void Update()
    {
        transform.position += InfiniteRunnerManager.Instance.scrollSpeed * Time.deltaTime * Vector3.left;

        if (transform.position.x < InfiniteRunnerManager.Instance.obstacleDespawnPoint.transform.position.x)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
