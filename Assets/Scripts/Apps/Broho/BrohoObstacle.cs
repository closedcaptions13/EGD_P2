using UnityEngine;

public class BrohoObstacle : MonoBehaviour
{
    public enum Kind
    {
        Linear,
        Wave
    }

    public Vector2 position;
    public Vector2 velocity;
    public float acceleration;
    public float waveAmplitude;
    public float waveRate;
    public Kind kind;

    void Update()
    {
        position += velocity * Time.deltaTime;
        velocity = (velocity.magnitude + Time.deltaTime * acceleration) * velocity.normalized;

        var realPosition = position;
        if (kind is Kind.Wave)
        {
            realPosition += Mathf.Sin(Time.time * waveRate) * waveAmplitude * new Vector2(-velocity.y, velocity.x).normalized;
        }

        transform.localEulerAngles = new(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            Vector2.SignedAngle(Vector2.up, velocity)
        );

        transform.localPosition = new(
            realPosition.x,
            realPosition.y,
            transform.localPosition.z);

        if (!BrohoManager.Instance.IsWithinLevel(transform.position))
        {
            GameObject.Destroy(gameObject);
        }
    }
}
