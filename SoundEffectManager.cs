using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager instance;
    [SerializeField] private AudioSource soundEffectObject;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

    public void PlaySoundEffect(AudioClip audioClip, float volume)
    {
        AudioSource audioSource = Instantiate(soundEffectObject, transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}
