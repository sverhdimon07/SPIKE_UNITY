using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDoorsSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;

    [SerializeField] private AudioSource audioSource1;
    public void PlaySound(int number)
    {
        audioSource1.PlayOneShot(sounds[number], 0.2f);
    }
}
