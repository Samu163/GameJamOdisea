using UnityEngine;

public class PlayerAudioSystem : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip ladridoClip;

    public void PlayLadrido()
    {
        audioSource.PlayOneShot(ladridoClip);
    }
}
