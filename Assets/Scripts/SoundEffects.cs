using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip buttonCastSound;
    public AudioClip buttonReelSound;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }

    public void PlayCastSound()
    {
        audioSource.PlayOneShot(buttonCastSound);
    }

    public void PlayReelSound()
    {
        audioSource.PlayOneShot(buttonReelSound);
    }
}