using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class ArenaEventWall : MonoBehaviour
{
    [SerializeField] private UnityEvent wallActivated;

    [SerializeField] private BoxCollider eventWallCollider;

    [SerializeField] private ParticleSystem dust1;
    [SerializeField] private ParticleSystem dust2;
    [SerializeField] private ParticleSystem fire1;
    [SerializeField] private ParticleSystem fire2;
    [SerializeField] private ParticleSystem fire3;
    [SerializeField] private ParticleSystem fire4;
    [SerializeField] private ParticleSystem fire5;

    private void OnTriggerExit(Collider collider)
    {
        if (!collider.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            return;
        }
        wallActivated.Invoke();
        TurningOnCollider();
    }

    private void TurningOnCollider()
    {
        eventWallCollider.isTrigger = false;
    }
}
