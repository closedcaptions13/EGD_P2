using UnityEngine;

public class PlayerLookaround : MonoBehaviour
{
    Quaternion startingRotation;
    [SerializeField] float viewRange;

    void Awake()
    {
        startingRotation = transform.rotation;
    }

    float xOnStartTab;
    float yOnStartTab;

    void Update()
    {
        var pos = Input.mousePosition;

        pos.x /= Screen.width;
        pos.y /= Screen.height;

        pos.x = pos.x * 2 - 1;
        pos.y = pos.y * 2 - 1;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            xOnStartTab = pos.x;
            yOnStartTab = pos.y;
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            pos.x = (pos.x - xOnStartTab) * 180 / viewRange + xOnStartTab;
            pos.y = (pos.y - yOnStartTab) * 40 / viewRange + yOnStartTab;
        }

        transform.rotation = Quaternion.Euler(pos.y * viewRange, pos.x * viewRange, 0) * startingRotation;
    }
}
