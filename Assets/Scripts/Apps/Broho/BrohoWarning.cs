using System;
using UnityEngine;

public class BrohoWarning : MonoBehaviour
{
    public Vector2 position;
    public float lifetime;
    public float angle;

    private float startTime;

    void Awake()
    {
        startTime = Time.time;
    }

    void Update()
    {
        transform.localPosition = new(
            position.x,
            position.y,
            transform.localPosition.z);

        transform.localEulerAngles = new(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            angle);

        if (Time.time - startTime > lifetime)
            GameObject.Destroy(gameObject);
    }
}
