using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource musicSource;
    public AudioClip backgroundMusic;

    private void Awake()
    {
        // if already exists, destroy this
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        //you get to live
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Start the music
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}
