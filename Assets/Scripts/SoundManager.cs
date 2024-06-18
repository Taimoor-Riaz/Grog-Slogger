using BugsBunny.Utilities.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSourceSettings backgroundMusicSettings;
    [SerializeField] private AudioSourceSettings sfxSettings;
    [SerializeField] private float changeMusicTime = 2.5f;

    private AudioSource backgroundSource;
    private AudioSource sfxSource;
    private Timer changeMusicTimer;

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<SoundManager>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        backgroundSource = backgroundMusicSettings.CreateAudioSource(gameObject);
        sfxSource = sfxSettings.CreateAudioSource(gameObject);

        gameObject.name = typeof(SoundManager).Name;
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void SetBackgroundMusic(AudioClip clip)
    {
        if (backgroundSource.clip == clip)
        {
            return;
        }
        if (changeMusicTimer != null)
        {
            changeMusicTimer.Stop();
        }
        if (backgroundSource == null || (!backgroundSource.isPlaying))
        {
            if (backgroundSource == null)
            {
                backgroundSource = backgroundMusicSettings.CreateAudioSource(gameObject);
            }
            backgroundSource.clip = clip;
            backgroundSource.Play();
        }
        else
        {
            AudioSource tempSource = backgroundSource;
            backgroundSource = backgroundMusicSettings.CreateAudioSource(gameObject);
            backgroundSource.clip = clip;
            backgroundSource.volume = 0f;
            backgroundSource.Play();

            changeMusicTimer = new Timer(changeMusicTime, () =>
            {
                Destroy(tempSource);
            });
            changeMusicTimer.AddOnUpdateAction((passedTime) =>
            {
                float t = passedTime / changeMusicTime;
                backgroundSource.volume = Mathf.Lerp(0f, backgroundMusicSettings.volume, t);
                tempSource.volume = backgroundMusicSettings.volume - t;
            });

            changeMusicTimer.SetOwner(gameObject);
            TimersManager.Add(changeMusicTimer);
        }
    }
}
[System.Serializable]
public class AudioSourceSettings
{
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
    public AudioSource CreateAudioSource(GameObject holder)
    {
        AudioSource audioSource = holder.AddComponent<AudioSource>();

        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.loop = loop;

        return audioSource;
    }
}
