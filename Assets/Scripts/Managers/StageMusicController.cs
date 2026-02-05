using UnityEngine;

public class StageMusicController : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip stageBGM;

    void Start()
    {
        // 1. Try to use the Global Audio Manager
        if (MainMenuAudioManager.Instance != null)
        {
            if (stageBGM != null)
            {
                MainMenuAudioManager.Instance.PlayMusic(stageBGM);
            }
        }
        else
        {
            // Fallback for testing directly in the scene
            Debug.LogWarning("StageMusicController: MainMenuAudioManager not found. Playing locally if AudioSource exists.");
            var source = GetComponent<AudioSource>();
            if (source != null && stageBGM != null)
            {
                source.clip = stageBGM;
                source.loop = true;
                source.Play();
            }
        }
    }
}
