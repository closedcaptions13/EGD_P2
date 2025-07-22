using UnityEngine;

public class InfiniteRunnerObstacle : MonoBehaviour
{
    void Update()
    {
        if (!InfiniteRunnerManager.IsPlaying)
        {
            GameObject.Destroy(gameObject);
        }

        if (InfiniteRunnerManager.IsPlaying)
        {
            transform.position += InfiniteRunnerManager.Instance.scrollSpeed * Time.deltaTime * Vector3.left;

            if (transform.position.x < InfiniteRunnerManager.Instance.obstacleDespawnPoint.transform.position.x)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
