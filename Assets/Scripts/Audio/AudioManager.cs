using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundClip
    {
        public string clipName;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;

        [HideInInspector]
        public AudioSource source;
    }

    [Header("Background Music")]
    public SoundClip[] backgroundMusic;

    [Header("Sound Effects")]
    public SoundClip[] soundEffects;

    [Header("UI Sounds")]
    public SoundClip[] uiSounds;

    // Track currently playing BGM
    private string currentBGM = "";
    private Dictionary<string, SoundClip> soundDictionary = new Dictionary<string, SoundClip>();

    private void Awake()
    {
        // Singleton pattern with proper scene transition handling
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize all sound clips
            InitializeAudioSources(backgroundMusic);
            InitializeAudioSources(soundEffects);
            InitializeAudioSources(uiSounds);
        }
        else
        {
            // This instance is a duplicate, destroy it
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources(SoundClip[] clips)
    {
        foreach (SoundClip s in clips)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            // Add to dictionary for easy lookup
            soundDictionary[s.clipName] = s;
        }
    }

    // Play a sound effect once
    public void PlaySound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundClip s))
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
    }

    // Play a sound effect at a specific position (for spatial audio)
    public void PlaySoundAtPosition(string soundName, Vector3 position)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundClip s))
        {
            AudioSource.PlayClipAtPoint(s.clip, position, s.volume);
        }
        else
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
    }

    // Play background music with crossfade
    public void PlayBackgroundMusic(string musicName, float fadeTime = 1.0f)
    {
        if (currentBGM == musicName) return;

        // Stop current BGM with fade out
        if (!string.IsNullOrEmpty(currentBGM) && soundDictionary.TryGetValue(currentBGM, out SoundClip currentMusic))
        {
            StartCoroutine(FadeOut(currentMusic.source, fadeTime));
        }

        // Start new BGM with fade in
        if (soundDictionary.TryGetValue(musicName, out SoundClip newMusic))
        {
            currentBGM = musicName;
            StartCoroutine(FadeIn(newMusic.source, fadeTime));
        }
        else
        {
            Debug.LogWarning("Background music: " + musicName + " not found!");
        }
    }

    // Stop a specific sound
    public void StopSound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundClip s))
        {
            s.source.Stop();
        }
    }

    // Stop all sounds
    public void StopAllSounds()
    {
        foreach (var sound in soundDictionary.Values)
        {
            sound.source.Stop();
        }
        currentBGM = "";
    }

    // Adjust volume for a specific sound
    public void SetVolume(string soundName, float volume)
    {
        volume = Mathf.Clamp01(volume);
        if (soundDictionary.TryGetValue(soundName, out SoundClip s))
        {
            s.volume = volume;
            s.source.volume = volume;
        }
    }

    // Adjust master volume
    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        AudioListener.volume = volume;
    }

    // Fade in coroutine
    private IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        // Get the target volume from the SoundClip
        float targetVolume = 0f;

        foreach (SoundClip clip in backgroundMusic)
        {
            if (clip.source == audioSource)
            {
                targetVolume = clip.volume;
                break;
            }
        }

        audioSource.volume = 0f;
        audioSource.Play();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    // Fade out coroutine
    private IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}