using TMPro;
using UnityEngine;

public class PopupError : MonoBehaviour
{
    public string Message { get; set; }

    [SerializeField] TextMeshProUGUI text;

    void Start()
    {
        text.text = Message;
    }

    public void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }
}
