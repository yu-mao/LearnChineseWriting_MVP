using UnityEngine;
using UnityEngine.UI;
public class VolumenControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource
    [SerializeField] private Slider volumeSlider;     // Reference to the Slider

    private void Start()
    {
        // Ensure the slider starts with the correct value
        if (audioSource != null && volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // This method is called whenever the slider's value changes
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    private void OnDestroy()
    {
        // Remove the listener to prevent memory leaks
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
        }
    }
}