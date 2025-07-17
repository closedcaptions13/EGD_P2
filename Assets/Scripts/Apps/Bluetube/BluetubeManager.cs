using System;
using UnityEngine;
using UnityEngine.Video;

public class BluetubeManager : MonoBehaviour
{
    [Serializable]
    public class Video
    {
        public string url;
        public string name;
    }

    public static BluetubeManager Instance { get; private set; }

    public VideoPlayer videoPlayer;
    public Video[] videos;
    public BluetubePlayButton buttonPrefab;
    public GameObject videoButtonHolder;

    void Awake()
    {
        Instance = this;

        foreach (var vid in videos)
        {
            var button = GameObject.Instantiate(buttonPrefab, videoButtonHolder.transform);

            button.videoURL = vid.url;
            button.videoName = vid.name;
        }
    }

    public void Play(string url)
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }

        videoPlayer.url = url;
        videoPlayer.Play();
    }
}