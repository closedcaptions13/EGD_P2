using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BluetubePlayButton : MonoBehaviour
{
    public string videoURL;
    public string videoName;

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(Play);

        var text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = videoName;
    }

    void Play()
    {
        BluetubeManager.Instance.Play(videoURL);
    }
}
