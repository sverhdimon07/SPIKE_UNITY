using UnityEngine;

public class TableSounds : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource1;
    public void PlaySound()
    {
        enabled = true;
        audioSource1.loop = true;
        audioSource1.Play();
    }
    public void StopSound()
    {
        enabled = false;
        audioSource1.Stop();
    }
 
}