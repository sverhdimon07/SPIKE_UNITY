using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDoorsSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;

    [SerializeField] private AudioSource audioSource1;
    public void PlaySound(int number)
    {
        float volumeScale = number == 0 ? 1.9f : 0.9f;
        audioSource1.PlayOneShot(sounds[number], volumeScale);
    }
}
