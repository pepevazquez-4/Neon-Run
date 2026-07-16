using UnityEngine;

public class MusicaManager : MonoBehaviour
{
    public AudioClip musicaFondo;
    [Range(0f, 1f)] public float volumen = 0.5f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicaFondo;
        audioSource.loop = true;
        audioSource.volume = volumen;
        audioSource.playOnAwake = false;

        if (musicaFondo != null)
        {
            audioSource.Play();
        }
    }
}