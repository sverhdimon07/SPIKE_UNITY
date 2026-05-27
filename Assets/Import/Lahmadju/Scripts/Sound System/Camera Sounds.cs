using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;

    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;
    [SerializeField] private AudioSource audioSource3;
    [SerializeField] private AudioSource audioSource4;

    public void PlaySound(int number)
    {
        audioSource1.pitch = 1;

        if (number == 0)
        {
            audioSource2.loop = true;
            audioSource2.Play();
        }
        else if (number == 3)
        {
            audioSource3.Play();
        }
        else if (number == 4)
        {
            audioSource4.Play();
            Invoke(nameof(StopPlayFourthAudioSource), 7f);
        }
        else if ((number == 5) || (number == 11))
        {
            audioSource1.pitch = Random.Range(0.9f, 1.15f);
            audioSource1.PlayOneShot(sounds[number], 0.1f);
        }
        else if (number == 13)
        {
            audioSource1.pitch = Random.Range(0.9f, 1.15f);
            audioSource1.PlayOneShot(sounds[number], 0.15f);
        }
        else if (number == 6)
        {
            audioSource1.PlayOneShot(sounds[number], 0.1f);
        }
        else if (number == 8)
        {
            Invoke(nameof(PlayEighthSound), 3f);
        }
        else if (number == 10)
        {
            audioSource1.PlayOneShot(sounds[number], 0.35f);
        }
        else if (number == 12)
        {
            audioSource1.PlayOneShot(sounds[number], 0.1f);
        }
        else if (number == 14)
        {
            audioSource1.PlayOneShot(sounds[number], 0.35f);
        }
        else if (number == 15)
        {
            audioSource1.PlayOneShot(sounds[number], 0.6f);
        }
        else
        {
            audioSource1.PlayOneShot(sounds[number], 0.25f);
        }
    }
    private void PlayEighthSound()
    {
        audioSource1.PlayOneShot(sounds[8], 0.25f);
    }
    public void ChangeVolumeOfSecondAuidoSource()
    {
        audioSource2.volume = audioSource2.volume - 0.02f;
    }
    public void StopPlaySecondAuidoSource()
    {
        audioSource2.Stop();
    }
    public void StopPlayThirdAuidoSource()
    {
        audioSource3.Stop();
    }
    public void StopPlayFourthAudioSource()
    {
        audioSource4.Stop();
    }
}