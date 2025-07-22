using UnityEngine;

public class PlayerLookaround : MonoBehaviour
{
    Quaternion startingRotation;
    [SerializeField] float viewRange;

    void Awake()
    {
        startingRotation = transform.rotation;
    }

    void Update()
    {
        var pos = Input.mousePosition;

        pos.x /= Screen.width;
        pos.y /= Screen.height;

        pos.x = pos.x * 2 - 1;
        pos.y = pos.y * 2 - 1;

        transform.rotation = Quaternion.Euler(pos.y * viewRange, pos.x * viewRange, 0) * startingRotation;
    }
}
