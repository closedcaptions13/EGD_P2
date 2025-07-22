using UnityEngine;

public class BrohoObstacle : MonoBehaviour
{
    public Vector2 position;
    public Vector2 velocity;
    public float acceleration;

    public Rigidbody2D Rigidbody { get; private set; }

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        transform.localPosition = position;
        Rigidbody.linearVelocity = velocity;
    }

    void Update()
    {
        if (!BrohoManager.Instance.IsPlaying)
            GameObject.Destroy(gameObject);

        if (!BrohoManager.Instance.IsWithinLevel(transform.position))
        {
            GameObject.Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        Rigidbody.linearVelocity = (Rigidbody.linearVelocity.magnitude + Time.fixedDeltaTime * acceleration) * Rigidbody.linearVelocity.normalized;

        transform.localEulerAngles = new(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            Vector2.SignedAngle(Vector2.up, velocity)
        );
    }
}
