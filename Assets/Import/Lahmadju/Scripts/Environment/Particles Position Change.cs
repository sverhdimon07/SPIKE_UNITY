using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPositionChange : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;

    private Vector3 additionalDistance = new Vector3(-0.5f, 1.5f, 0.75f);

    [SerializeField] private Transform particleTransform;
    [SerializeField] private Transform pointTransform;
    public void PlayParticles()
    {
        particleTransform.position = pointTransform.position + additionalDistance;
        particle.Play();
    }
}