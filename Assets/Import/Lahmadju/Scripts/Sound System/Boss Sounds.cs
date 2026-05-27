using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;

    [SerializeField] private AudioSource audioSource1;
    public void PlaySound(int number)
    {
        audioSource1.pitch = 1f;

        if (number == 4)
        {
            audioSource1.pitch = Random.Range(0.95f, 1.1f);
            audioSource1.PlayOneShot(sounds[number], 0.25f);
        }
        else if (number == 7)
        {
            audioSource1.PlayOneShot(sounds[number], 0.08f);
        }
        else
        {
            audioSource1.PlayOneShot(sounds[number], 0.2f);
        }
    }
}