using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum SoundType
{
    FANGENERAL,
    FANLECTURE,
    LECTURE,
    STARTQUIZ,
}

public enum SpecificBarks
{
    GENERICBARK1,
    GENERICBARK2,
    GENERICBARK3,
    JARED_BARK,
    JAS_BARK,
    JOE_BARK,
    JUSTIN_BARK,
    KEVIN_BARK,
    LAMAR_BARK,
    LINUS_BARK,
    LYLE_BARK,
    MATT_BARK,
    PROFSILVIA_BARK,
    RY_BARK,
    TANOAH_BARK
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] private AudioClip[] soundList;
    [SerializeField] private AudioClip[] barkList;
    [SerializeField] private AudioSource lectureAudioSource;
    [SerializeField] private AudioSource generalAudioSource;
    [SerializeField] private AudioSource fanAudioSource;

    private void Start()
    {
        instance.fanAudioSource.clip = instance.soundList[(int)SoundType.FANGENERAL];
        instance.fanAudioSource.Play();
        // instance.StartCoroutine(instance.FadeOut(instance.fanAudioSource, 1.5f));
    }

    private void Awake()
    {
        instance = this;
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        if (sound == SoundType.LECTURE)
        {
            instance.StartCoroutine(instance.FadeOut(instance.fanAudioSource, 1.5f));

            instance.lectureAudioSource.PlayOneShot(instance.soundList[(int)sound], 1);
        }
        else
        {
            instance.generalAudioSource.PlayOneShot(instance.soundList[(int)sound], volume);

            if (sound == SoundType.STARTQUIZ)
            {
                instance.fanAudioSource.PlayOneShot(instance.soundList[(int)SoundType.FANGENERAL], 1);

                instance.StartCoroutine(instance.FadeIn(instance.fanAudioSource, 1.5f));
            }
        }
    }

    public static float TimeInLecture
        => instance.lectureAudioSource.time;
    public static bool LectureIsFinished
        => instance
        && instance.lectureAudioSource.clip == instance.soundList[(int)SoundType.LECTURE]
        && !instance.lectureAudioSource.isPlaying;

    public static void PlaySound(int sound)
    {
        if (sound == 2)
        {
            // instance.fanAudioSource.Stop();
            // instance.fanAudioSource.clip = instance.soundList[(int)SoundType.FANLECTURE];
            // instance.fanAudioSource.Play();

            // instance.fanAudioSource.volume = 0.05f;
            instance.StartCoroutine(instance.FadeOut(instance.fanAudioSource, 1.5f));

            instance.lectureAudioSource.PlayOneShot(instance.soundList[(int)sound], 1);
        }
        else
        {
            instance.generalAudioSource.PlayOneShot(instance.soundList[sound], 1);

            if (sound == 3)
            {
                // instance.fanAudioSource.Stop();
                // instance.fanAudioSource.PlayOneShot(instance.soundList[(int)SoundType.FANGENERAL], 1);

                instance.StartCoroutine(instance.FadeIn(instance.fanAudioSource, 2f));
            }
        }
    }

    public static void PlayBark(SoundType sound)
    {
        PauseLecture();
        instance.StartCoroutine(instance.InterruptLecture(instance.generalAudioSource, instance.barkList[(int)sound]));
    }

    public static void PlayBark(int sound)
    {
        PauseLecture();
        instance.StartCoroutine(instance.InterruptLecture(instance.generalAudioSource, instance.barkList[(int)sound]));
    }
    
    public static async UniTask PlayBarkAsync(int sound)
    {
        PauseLecture();
       await instance.InterruptLecture(instance.generalAudioSource, instance.barkList[(int)sound]);
    }

    public static void PauseLecture()
    {
        instance.lectureAudioSource.Pause();
    }

    public static void ResumeLecture()
    {
        // Debug.Log("Lecture resumed");
        instance.lectureAudioSource.UnPause();
    }

    public static void ChangeLectureSpeed(float newSpeed)
    {
        instance.lectureAudioSource.pitch = newSpeed;
    }

    public IEnumerator InterruptLecture(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();

        while (source.isPlaying)
        {
            yield return null;
        }

        ResumeLecture();
    }

    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0.05)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }
    }
    
    public IEnumerator FadeIn(AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;
        
        while (audioSource.volume < 1)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }
    }
}
