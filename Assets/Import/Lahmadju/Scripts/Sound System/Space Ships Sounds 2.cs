using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipsSounds2 : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource1;
    private void Start()
    {
        Invoke(nameof(Playing), 3.8f);
    }
    private void Playing()
    {
        audioSource1.Play();
    }
}
