using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;

    [Header("Toggle Buttons")]
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle musicToggle;

    [Header("Test Sound Buttons")]
    [SerializeField] private Button testSfxButton;
    [SerializeField] private Button testMusicButton;
    [SerializeField] private Button testUIButton;

    [Header("Sound Test References")]
    [SerializeField] private string testSfxSound = "testSfx";
    [SerializeField] private string testMusicSound = "testMusic";
    [SerializeField] private string testUISound = "testUI";

    // Default values
    private const float DEFAULT_VOLUME = 0.75f;
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SfxVolume";
    private const string UI_VOLUME_KEY = "UIVolume";
    private const string SOUND_ENABLED_KEY = "SoundEnabled";
    private const string MUSIC_ENABLED_KEY = "MusicEnabled";

    private void Start()
    {
        // Load saved settings
        LoadSettings();

        // Set up UI listeners
        SetupUIListeners();
    }

    private void LoadSettings()
    {
        // Set up slider values
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, DEFAULT_VOLUME);

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_VOLUME);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_VOLUME);

        if (uiVolumeSlider != null)
            uiVolumeSlider.value = PlayerPrefs.GetFloat(UI_VOLUME_KEY, DEFAULT_VOLUME);

        // Set up toggle values
        if (soundToggle != null)
            soundToggle.isOn = PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;

        if (musicToggle != null)
            musicToggle.isOn = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;

        // Apply settings to AudioManager
        ApplySettings();
    }

    private void SetupUIListeners()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

        if (uiVolumeSlider != null)
            uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);

        if (soundToggle != null)
            soundToggle.onValueChanged.AddListener(OnSoundEnabledChanged);

        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicEnabledChanged);

        // Set up test buttons
        if (testSfxButton != null)
            testSfxButton.onClick.AddListener(TestSfxSound);

        if (testMusicButton != null)
            testMusicButton.onClick.AddListener(TestMusicSound);

        if (testUIButton != null)
            testUIButton.onClick.AddListener(TestUISound);
    }

    private void ApplySettings()
    {
        if (AudioManager.Instance == null) return;

        // Get saved values
        float masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, DEFAULT_VOLUME);
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_VOLUME);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_VOLUME);
        float uiVolume = PlayerPrefs.GetFloat(UI_VOLUME_KEY, DEFAULT_VOLUME);
        bool soundEnabled = PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;
        bool musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;

        // Apply master volume
        AudioManager.Instance.SetMasterVolume(soundEnabled ? masterVolume : 0f);

        // Apply volume for each background music
        foreach (var music in AudioManager.Instance.backgroundMusic)
        {
            AudioManager.Instance.SetVolume(music.clipName, musicEnabled ? musicVolume : 0f);
        }

        // Apply volume for SFX and UI
        foreach (var sfx in AudioManager.Instance.soundEffects)
        {
            AudioManager.Instance.SetVolume(sfx.clipName, soundEnabled ? sfxVolume : 0f);
        }

        foreach (var ui in AudioManager.Instance.uiSounds)
        {
            AudioManager.Instance.SetVolume(ui.clipName, soundEnabled ? uiVolume : 0f);
        }
    }

    // Slider event handlers
    private void OnMasterVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
        ApplySettings();
    }

    private void OnMusicVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        ApplySettings();
    }

    private void OnSfxVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        ApplySettings();
    }

    private void OnUIVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat(UI_VOLUME_KEY, volume);
        ApplySettings();
    }

    // Toggle event handlers
    private void OnSoundEnabledChanged(bool enabled)
    {
        PlayerPrefs.SetInt(SOUND_ENABLED_KEY, enabled ? 1 : 0);
        ApplySettings();
    }

    private void OnMusicEnabledChanged(bool enabled)
    {
        PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, enabled ? 1 : 0);
        ApplySettings();
    }

    // Test sound methods
    private void TestSfxSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(testSfxSound);
        }
    }

    private void TestMusicSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(testMusicSound);
        }
    }

    private void TestUISound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(testUISound);
        }
    }
}