using UnityEngine;

public enum SoundType
{
    FANGENERAL,
    FANLECTURE,
    LECTURE,
    GENERICBARK1,
    GENERICBARK2,
    GENERICBARK3,
    STARTQUIZ,
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static AudioManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
