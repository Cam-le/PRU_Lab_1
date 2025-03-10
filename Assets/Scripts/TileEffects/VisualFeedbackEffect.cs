using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Effect that provides visual and audio feedback without gameplay changes
/// Useful for decorative tiles or to enhance other effects
/// </summary>
public class VisualFeedbackEffect : TileEffect
{
    public enum FeedbackType
    {
        Particles,
        Animation,
        ScreenShake,
        SoundEffect,
        CameraZoom,
        ColorFlash,
        Combined
    }

    [System.Serializable]
    public class ParticleSettings
    {
        public GameObject particlePrefab;
        public float duration = 2f;
        public float emissionRate = 10f;
        public Color particleColor = Color.white;
        public Vector3 offset = Vector3.zero;
    }

    [System.Serializable]
    public class AnimationSettings
    {
        public string animationTrigger = "TriggerEffect";
        public float duration = 1f;
        public bool affectPlayer = true;
        public bool affectTile = true;
    }

    [System.Serializable]
    public class ScreenShakeSettings
    {
        public float duration = 0.5f;
        public float magnitude = 0.1f;
        public float noiseFrequency = 10f;
    }

    [System.Serializable]
    public class CameraZoomSettings
    {
        public float targetZoom = 3f;
        public float zoomDuration = 1f;
        public float holdDuration = 0.5f;
        public AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    [System.Serializable]
    public class ColorFlashSettings
    {
        public Color flashColor = Color.white;
        public float duration = 0.2f;
        public int flashCount = 1;
        public bool affectBackground = true;
        public bool affectTile = true;
    }

    [Header("Feedback Settings")]
    [SerializeField] private FeedbackType feedbackType = FeedbackType.Particles;
    [SerializeField] private List<FeedbackType> combinedFeedback = new List<FeedbackType>();
    [SerializeField] private bool delayBetweenEffects = false;
    [SerializeField] private float delayDuration = 0.2f;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSettings particleSettings = new ParticleSettings();

    [Header("Animation Settings")]
    [SerializeField] private AnimationSettings animationSettings = new AnimationSettings();

    [Header("Screen Shake Settings")]
    [SerializeField] private ScreenShakeSettings shakeSettings = new ScreenShakeSettings();

    [Header("Sound Settings")]
    [SerializeField] private List<AudioClip> soundEffects = new List<AudioClip>();
    [SerializeField] private bool playRandomSound = true;
    [SerializeField] private int specificSoundIndex = 0;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float pitch = 1f;
    [SerializeField] private bool spatialSound = false;

    [Header("Camera Zoom Settings")]
    [SerializeField] private CameraZoomSettings zoomSettings = new CameraZoomSettings();

    [Header("Color Flash Settings")]
    [SerializeField] private ColorFlashSettings flashSettings = new ColorFlashSettings();

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public override bool ApplyEffect(PlayerController player)
    {
        // Call base implementation for common effect handling
        base.ApplyEffect(player);

        // Apply the appropriate visual feedback
        if (feedbackType == FeedbackType.Combined)
        {
            if (delayBetweenEffects)
            {
                StartCoroutine(ApplyCombinedFeedbackWithDelay(player));
            }
            else
            {
                ApplyCombinedFeedback(player);
            }
        }
        else
        {
            ApplySpecificFeedback(feedbackType, player);
        }

        return true;
    }

    private void ApplyCombinedFeedback(PlayerController player)
    {
        foreach (FeedbackType type in combinedFeedback)
        {
            ApplySpecificFeedback(type, player);
        }
    }

    private IEnumerator ApplyCombinedFeedbackWithDelay(PlayerController player)
    {
        foreach (FeedbackType type in combinedFeedback)
        {
            ApplySpecificFeedback(type, player);
            yield return new WaitForSeconds(delayDuration);
        }
    }

    private void ApplySpecificFeedback(FeedbackType type, PlayerController player)
    {
        switch (type)
        {
            case FeedbackType.Particles:
                ApplyParticleEffect(player.transform.position);
                break;
            case FeedbackType.Animation:
                ApplyAnimationEffect(player);
                break;
            case FeedbackType.ScreenShake:
                StartCoroutine(ApplyScreenShake());
                break;
            case FeedbackType.SoundEffect:
                ApplySoundEffect(player.transform.position);
                break;
            case FeedbackType.CameraZoom:
                StartCoroutine(ApplyCameraZoom());
                break;
            case FeedbackType.ColorFlash:
                StartCoroutine(ApplyColorFlash());
                break;
        }
    }

    private void ApplyParticleEffect(Vector3 position)
    {
        if (particleSettings.particlePrefab == null) return;

        Vector3 spawnPosition = position + particleSettings.offset;
        GameObject particleObj = Instantiate(particleSettings.particlePrefab, spawnPosition, Quaternion.identity);

        ParticleSystem particleSystem = particleObj.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startColor = particleSettings.particleColor;

            var emission = particleSystem.emission;
            emission.rateOverTime = particleSettings.emissionRate;

            Destroy(particleObj, particleSettings.duration);
        }
        else
        {
            Destroy(particleObj, 2f); // Default fallback
        }
    }

    private void ApplyAnimationEffect(PlayerController player)
    {
        // Apply to player if specified
        if (animationSettings.affectPlayer)
        {
            Animator playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(animationSettings.animationTrigger);
            }
        }

        // Apply to tile if specified
        if (animationSettings.affectTile)
        {
            Tile currentTile = FindObjectOfType<GridManager>()?.GetTileAtIndex(PlayerState.CurrentTileIndex);
            if (currentTile != null)
            {
                Animator tileAnimator = currentTile.GetComponent<Animator>();
                if (tileAnimator != null)
                {
                    tileAnimator.SetTrigger(animationSettings.animationTrigger);
                }
            }
        }
    }

    private void ApplySoundEffect(Vector3 position)
    {
        if (soundEffects.Count == 0) return;

        // Determine which sound to play
        AudioClip clipToPlay;
        if (playRandomSound)
        {
            int randomIndex = Random.Range(0, soundEffects.Count);
            clipToPlay = soundEffects[randomIndex];
        }
        else
        {
            int index = Mathf.Clamp(specificSoundIndex, 0, soundEffects.Count - 1);
            clipToPlay = soundEffects[index];
        }

        if (clipToPlay == null) return;

        // Play the sound
        if (AudioManager.Instance != null)
        {
            if (spatialSound)
            {
                AudioSource.PlayClipAtPoint(clipToPlay, position, volume);
            }
            else
            {
                AudioManager.Instance.PlaySound(clipToPlay.name);
            }
        }
    }

    private IEnumerator ApplyScreenShake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) yield break;

        Vector3 originalPosition = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeSettings.duration)
        {
            float xOffset = Random.Range(-1f, 1f) * shakeSettings.magnitude;
            float yOffset = Random.Range(-1f, 1f) * shakeSettings.magnitude;

            mainCamera.transform.position = new Vector3(
                originalPosition.x + xOffset,
                originalPosition.y + yOffset,
                originalPosition.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original position
        mainCamera.transform.position = originalPosition;
    }

    private IEnumerator ApplyCameraZoom()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null || !mainCamera.orthographic) yield break;

        float originalSize = mainCamera.orthographicSize;
        float elapsed = 0f;

        // Zoom in
        while (elapsed < zoomSettings.zoomDuration)
        {
            float t = elapsed / zoomSettings.zoomDuration;
            float evaluatedT = zoomSettings.zoomCurve.Evaluate(t);

            mainCamera.orthographicSize = Mathf.Lerp(originalSize, zoomSettings.targetZoom, evaluatedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hold at zoomed level
        mainCamera.orthographicSize = zoomSettings.targetZoom;
        yield return new WaitForSeconds(zoomSettings.holdDuration);

        // Zoom back out
        elapsed = 0f;
        while (elapsed < zoomSettings.zoomDuration)
        {
            float t = elapsed / zoomSettings.zoomDuration;
            float evaluatedT = zoomSettings.zoomCurve.Evaluate(t);

            mainCamera.orthographicSize = Mathf.Lerp(zoomSettings.targetZoom, originalSize, evaluatedT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original size
        mainCamera.orthographicSize = originalSize;
    }

    private IEnumerator ApplyColorFlash()
    {
        if (flashSettings.flashCount <= 0) yield break;

        // Get the tile to affect
        Tile currentTile = null;
        if (flashSettings.affectTile)
        {
            currentTile = FindObjectOfType<GridManager>()?.GetTileAtIndex(PlayerState.CurrentTileIndex);
        }

        // Get background to affect
        SpriteRenderer backgroundRenderer = null;
        if (flashSettings.affectBackground)
        {
            GameObject background = GameObject.FindGameObjectWithTag("Background");
            if (background != null)
            {
                backgroundRenderer = background.GetComponent<SpriteRenderer>();
            }
        }

        // Cache original colors
        Color originalTileColor = Color.white;
        if (currentTile != null)
        {
            SpriteRenderer tileRenderer = currentTile.GetComponent<SpriteRenderer>();
            if (tileRenderer != null)
            {
                originalTileColor = tileRenderer.color;
            }
        }

        Color originalBgColor = Color.white;
        if (backgroundRenderer != null)
        {
            originalBgColor = backgroundRenderer.color;
        }

        // Perform the flash sequence
        for (int i = 0; i < flashSettings.flashCount; i++)
        {
            // Flash on
            if (currentTile != null)
            {
                SpriteRenderer tileRenderer = currentTile.GetComponent<SpriteRenderer>();
                if (tileRenderer != null)
                {
                    tileRenderer.color = flashSettings.flashColor;
                }
            }

            if (backgroundRenderer != null)
            {
                backgroundRenderer.color = flashSettings.flashColor;
            }

            yield return new WaitForSeconds(flashSettings.duration / 2);

            // Flash off
            if (currentTile != null)
            {
                SpriteRenderer tileRenderer = currentTile.GetComponent<SpriteRenderer>();
                if (tileRenderer != null)
                {
                    tileRenderer.color = originalTileColor;
                }
            }

            if (backgroundRenderer != null)
            {
                backgroundRenderer.color = originalBgColor;
            }

            if (i < flashSettings.flashCount - 1)
            {
                yield return new WaitForSeconds(flashSettings.duration / 2);
            }
        }
    }

    public override string GetEffectSummary()
    {
        if (feedbackType == FeedbackType.Combined)
        {
            return $"Visual Effects: {combinedFeedback.Count} combined effects";
        }
        else
        {
            return $"Visual Effect: {feedbackType}";
        }
    }
}