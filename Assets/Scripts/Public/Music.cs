using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public float DefaultVolume;
    public float fadeTime;
    public AudioClip splashMusic;
    public AudioClip backgroundMusic;
    public AudioClip menuMusic;

    public static Music instance;

    private static bool isMusicPlaying = false;

    private AudioSource _audioSource;

    public enum PipesMusics
    {
        Splash,
        Menu,
        Gameplay
    }

    // Start is called before the first frame update
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        StartMusic(PipesMusics.Splash);
    }

    public void StartMusic(PipesMusics theMusic, bool changeClip = false)
    {
        StartCoroutine(StartMusicIE(theMusic, changeClip));
    }

    private IEnumerator StartMusicIE(PipesMusics theMusic, bool changeClip = false)
    {
        if (Manager.instance.isMusicEnabled && ( !isMusicPlaying || changeClip))
        {
            if (changeClip)
            {
                yield return StartCoroutine(FadeOutMusic(_audioSource, DefaultVolume, fadeTime));
                _audioSource.Stop();
            }
            StartCoroutine(FadeInMusic(_audioSource, DefaultVolume, fadeTime));
            _audioSource.clip = GetMusicClip(theMusic);
            _audioSource.Play();
            isMusicPlaying = !(theMusic == PipesMusics.Splash);
            _audioSource.loop = !(theMusic == PipesMusics.Splash);
        }
    }

    public void StopMusic()
    {
        StartCoroutine(StopMusicIE());
    }

    private IEnumerator StopMusicIE()
    {
        yield return StartCoroutine(FadeOutMusic(_audioSource, DefaultVolume, fadeTime));
        _audioSource.Stop();
        isMusicPlaying = false;
    }

    public void MuteMusic()
    {
        StartCoroutine(MuteMusicIE());
    }

    private IEnumerator MuteMusicIE()
    {
        yield return StartCoroutine(FadeOutMusic(_audioSource, DefaultVolume, fadeTime));
        _audioSource.mute = true;
    }

    public void UnmuteMusic()
    {
        StartCoroutine(FadeInMusic(_audioSource, DefaultVolume, fadeTime));
        _audioSource.mute = false;
    }

    public static IEnumerator FadeInMusic(AudioSource theAudioSource, float DefaultVolume, float fadeTime)
    {
        theAudioSource.volume = 0f;
        float currentTime = 0f;
        do
        {
            theAudioSource.volume = Mathf.SmoothStep(0f, DefaultVolume, currentTime / fadeTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= fadeTime);
        theAudioSource.volume = DefaultVolume;
    }

    public static IEnumerator FadeOutMusic(AudioSource theAudioSource, float DefaultVolume, float fadeTime)
    {
        theAudioSource.volume = DefaultVolume;
        float currentTime = 0f;
        do
        {
            theAudioSource.volume = Mathf.SmoothStep(DefaultVolume, 0f, currentTime / fadeTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= fadeTime);
        theAudioSource.volume = 0f;
    }

    private AudioClip GetMusicClip(PipesMusics theMusic)
    {
        switch (theMusic)
        {
            case PipesMusics.Splash:
                return splashMusic;
            case PipesMusics.Menu:
                return menuMusic;
            case PipesMusics.Gameplay:
                return backgroundMusic;
            default:
                return backgroundMusic;
        }
    }
}
