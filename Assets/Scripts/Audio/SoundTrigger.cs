using UnityEngine;
using UnityEngine.Events;

public class SoundTrigger : MonoBehaviour
{
    // Sound to play
    [SerializeField] private string soundName;

    // Whether to play sound at this transform's position (spatial audio)
    [SerializeField] private bool playAtPosition = false;

    // When to trigger the sound
    public enum TriggerType
    {
        OnStart,
        OnEnable,
        OnDisable,
        OnTriggerEnter,
        OnTriggerExit,
        OnCollisionEnter,
        OnCollisionExit,
        OnPlayerLand,
        Manual
    }

    [SerializeField] private TriggerType triggerType = TriggerType.Manual;

    // For manual triggering
    public UnityEvent onSoundTriggered;

    private void Start()
    {
        if (triggerType == TriggerType.OnStart)
        {
            PlaySound();
        }
    }

    private void OnEnable()
    {
        if (triggerType == TriggerType.OnEnable)
        {
            PlaySound();
        }
    }

    private void OnDisable()
    {
        if (triggerType == TriggerType.OnDisable)
        {
            PlaySound();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerType == TriggerType.OnTriggerEnter && other.CompareTag("Player"))
        {
            PlaySound();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerType == TriggerType.OnTriggerExit && other.CompareTag("Player"))
        {
            PlaySound();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (triggerType == TriggerType.OnCollisionEnter && collision.gameObject.CompareTag("Player"))
        {
            PlaySound();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (triggerType == TriggerType.OnCollisionExit && collision.gameObject.CompareTag("Player"))
        {
            PlaySound();
        }
    }

    // Public method for external triggering
    public void PlaySound()
    {
        if (string.IsNullOrEmpty(soundName)) return;

        if (AudioManager.Instance != null)
        {
            if (playAtPosition)
            {
                AudioManager.Instance.PlaySoundAtPosition(soundName, transform.position);
            }
            else
            {
                AudioManager.Instance.PlaySound(soundName);
            }

            onSoundTriggered?.Invoke();
        }
    }
}