using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class InfiniteRunnerPlayer : MonoBehaviour
{
    public Rigidbody2D Rigidbody { get; private set; }

    [SerializeField] float jumpForce;
    [SerializeField] float jetpackForce;

    public enum JetpackState
    {
        Grounded,
        JustJumped,
        Usable
    }

    bool isGrounded;
    JetpackState jetpack;

    private RigidbodyConstraints2D originalConstraints;

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private TMP_Text finalTime;

    void Start()
    {
        originalConstraints = GetComponent<Rigidbody2D>().constraints;
        deathScreen.SetActive(false);
    }

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isGrounded)
        {
            jetpack = JetpackState.Grounded;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jetpack = JetpackState.JustJumped;
                isGrounded = false;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (jetpack is JetpackState.Usable)
            {
                Rigidbody.AddForce(Vector2.up * jetpackForce);
            }
        }
        else
        {
            if (jetpack is JetpackState.JustJumped)
            {
                jetpack = JetpackState.Usable;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var isDeath =
            Vector2.Dot(collision.GetContact(0).normal, Vector2.up) < 0.5;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }

        if (isDeath || collision.gameObject.layer == LayerMask.NameToLayer("Damage"))
        {
            Debug.Log($"{Time.time} :: you died fucking idiot kys");

            deathScreen.SetActive(true);
            finalTime.text = $"Final Time: {Time.timeSinceLevelLoad.ToString("F2")}";
            Time.timeScale = 0;
            Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = false;
    }

    public void Restart()
    {
        Rigidbody.constraints = originalConstraints;
        Time.timeScale = 1;
        SceneManager.LoadScene("Infinite Runner");
        // deathScreen.SetActive(false);
        
    }
}
