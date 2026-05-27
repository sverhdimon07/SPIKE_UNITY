using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipsSounds : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource1;
    private void Start()
    {
        audioSource1.Play();
    }
}
