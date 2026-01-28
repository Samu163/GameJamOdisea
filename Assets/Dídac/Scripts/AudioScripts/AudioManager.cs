using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    [Header("Music Settings")]
    public AudioSource titelMusic;
    public AudioSource gameMusic;
    public List<AudioClip> levelClips;
    private bool isTitelMusicPlaying = true;
    private bool isGameMusicPlaying = false;

    [Header("Player SFX")]
    [Header("Footsteps SFX")]
    public AudioSource footstepsSource;
    public List<AudioClip> footstepClips;
    private bool isKeyboardFootstepPlaying = false;

    [Header("Ability SFX")]
    public AudioSource abilitySource;
    public AudioClip startAbilityClip;
    public List<AudioClip> endAbilityClips;

    [Header("Guau SFX")]
    public AudioSource guauSource;
    public List<AudioClip> guauClips;

    [Header("Death SFX")]
    public AudioSource deathSource;

    [Header("Talking SFX")]
    public AudioSource talkingSource;
    public AudioObject talkingAudio;

    [Header("Box SFX")]
    public AudioSource boxSource;
    public List<AudioClip> boxClips;

    [Header("Puzzle SFX")]
    [Header("Puzzle Completed SFX")]
    public AudioSource puzzleCompletedSource;

    [Header("Palanca SFX")]
    public AudioSource palancaSource;

    [Header("Placa de Pressión SFX")]
    public AudioSource placaSource;

    [Header("Rudder SFX")]
    public AudioSource rudderSource;

    [Header("Puente SFX")]
    public AudioSource puenteSource;

    [Header("Laser SFX")]
    public AudioSource laserSource;

    [Header("Traps SFX")]
    [Header("Parrilla Fuego SFX")]
    public AudioSource parrillaFuegoSource;

    [Header("Pinchos SFX")]
    public AudioSource pinchosSource;

    [Header("Ballesta SFX")]
    public AudioSource ballestaSource;

    [Header("UI Hover SFX")]
    public AudioSource uiHoverSource;

    [Header("UI Confirm SFX")]
    public AudioSource uiConfirmSource;

    [Header("UI Decline SFX")]
    public AudioSource uiDeclinesource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isKeyboardFootstepPlaying)
        {
            if (!footstepsSource.isPlaying)
            {
                int index = Random.Range(0, footstepClips.Count);
                footstepsSource.PlayOneShot(footstepClips[index]);
            }
        }
    }

    public void PlayTitleMusic()
    {
        if (!isTitelMusicPlaying)
        {
            StartCoroutine(FadeOutCoroutine(1f, gameMusic, titelMusic));
            isGameMusicPlaying = false;
            isTitelMusicPlaying = true;
        }
    }

    public void PlayTemple1Music()
    {
        if (!isGameMusicPlaying)
        {
            StartCoroutine(FadeOutCoroutine(1f, titelMusic, gameMusic));
            isTitelMusicPlaying = false;
            gameMusic.clip = levelClips[0];
            isGameMusicPlaying = true;
        }
    }

    public void PlayTemple2Music()
    {
        if (!isGameMusicPlaying)
        {
            StartCoroutine(FadeOutCoroutine(1f, titelMusic, gameMusic));
            isTitelMusicPlaying = false;
            gameMusic.clip = levelClips[1];
            isGameMusicPlaying = true;
        }
    }

    public void PlayTemple3Music()
    {
        if (!isGameMusicPlaying)
        {
            StartCoroutine(FadeOutCoroutine(1f, titelMusic, gameMusic));
            isTitelMusicPlaying = false;
            gameMusic.clip = levelClips[2];
            isGameMusicPlaying = true;
        }
    }

    private IEnumerator FadeOutCoroutine(float duration, AudioSource audioSource, AudioSource toPlay)
    {
        float startVolume = audioSource.volume;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, timeElapsed / duration);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
        toPlay.Play();

        audioSource.volume = startVolume;
    }

    public void PlayFootstepGamepad()
    {
        if (!footstepsSource.isPlaying)
        {
            int index = Random.Range(0, footstepClips.Count);
            footstepsSource.PlayOneShot(footstepClips[index]);
        }
        
    }

    public void PlayFootstepKeyboard()
    {
        isKeyboardFootstepPlaying = true;
    }

    public void StopFootsteps()
    {
        isKeyboardFootstepPlaying = false;
        footstepsSource.Stop();
    }

    public void PlayTalking(float pitch = 1f)
    {
        talkingSource.pitch = pitch;
        talkingSource.PlayOneShot(talkingAudio.GetRandomAudio().clip);
        //int index = Random.Range(0, talkingClips.Count);
        //talkingSource.PlayOneShot(talkingClips[index]);
    }

    public void StopTalking()
    {
        talkingSource.Stop();
    }

    public void PlayGuau()
    {
        if (guauSource.isPlaying) return;
        int index = Random.Range(0, guauClips.Count);
        guauSource.PlayOneShot(guauClips[index]);
    }

    public void PlayCogerBox()
    {
        boxSource.PlayOneShot(boxClips[0]);
    }

    public void PlaySoltarBox()
    {
        boxSource.PlayOneShot(boxClips[1]);
    }

    public void PlayAbility()
    {
        abilitySource.PlayOneShot(startAbilityClip);
    }

    public void EndAbility()
    {
        abilitySource.Stop();
        //int index = Random.Range(0, endAbilityClips.Count);
        abilitySource.Stop();
    }

    public void PlayDeath()
    {
        deathSource.Play();
    }

    public void PlayPuzzleCompleted()
    {
        puzzleCompletedSource.Play();
    }

    public void PlayPalanca()
    {
        palancaSource.Play();
    }

    public void PlayPlaca()
    {
        placaSource.Play();
    }

    public void PlayRudder()
    {
        rudderSource.Play();
    }

    public void PlayPuente()
    {
        puenteSource.Play();
    }

    public void PlayLaser()
    {
        laserSource.Play();
    }

    public void PlayParrillaFuego()
    {
        parrillaFuegoSource.Play();
    }

    public void PlayPinchos()
    {
        pinchosSource.Play();
    }

    public void PlayBallesta()
    {
        ballestaSource.Play();
    }


    // Este audio manager es una terroristada
    public void PlayUIHoverSfx()
    {
       uiHoverSource.Play();
    }

    public void PlayUIConfirmSfx()
    {

       uiConfirmSource.Play();
    }

    public void PlayUIDeclineSfx()
    {

       uiDeclinesource.Play();
    }

}
