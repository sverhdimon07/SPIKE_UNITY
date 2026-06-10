using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class ArenaEventWall : MonoBehaviour
{
    [SerializeField] private UnityEvent wallActivated;

    [SerializeField] private ParticleSystem dust1;
    [SerializeField] private ParticleSystem dust2;
    [SerializeField] private ParticleSystem fire1;
    [SerializeField] private ParticleSystem fire2;
    [SerializeField] private ParticleSystem fire3;
    [SerializeField] private ParticleSystem fire4;
    [SerializeField] private ParticleSystem fire5;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<PlayerController>() == false)
        {
            print("!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return;
        }
        wallActivated.Invoke();
        print("AAAAAAAAAAAAAAAAAAAAa");
    }
}
