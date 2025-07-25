using TMPro;
using UnityEngine;

public class BrohoPlayer : MonoBehaviour
{
    public float moveSpeed;

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private TMP_Text finalTime;

    void Start()
    {
        deathScreen.SetActive(false);
    }

    void Update()
    {
        var velocity = Vector2.zero;

        if (AppRoot.SelectedForObject(this))
        {
            if (Input.GetKey(KeyCode.W))
                velocity += Vector2.up;
            if (Input.GetKey(KeyCode.A))
                velocity += Vector2.left;
            if (Input.GetKey(KeyCode.S))
                velocity += Vector2.down;
            if (Input.GetKey(KeyCode.D))
                velocity += Vector2.right;
        }

        velocity.Normalize();
        velocity *= moveSpeed * Time.deltaTime;

        transform.position += (Vector3)velocity;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Damage"))
        {
            BrohoManager.Instance.KillGameplay();
            Death();
        }
    }

    void Death()
    {
        finalTime.text = $"Final Time: {BrohoManager.Instance.GetCurrTime().ToString("F2")}";
        deathScreen.SetActive(true);
        BrohoManager.Instance.KillGameplay();
    }

    public void Restart()
    {
        deathScreen.SetActive(false);
        BrohoManager.Instance.BeginGameplay();

    }
}
