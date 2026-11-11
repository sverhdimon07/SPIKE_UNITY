using UnityEngine;

public sealed class FlightLimiter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.LockFighterForm();
        }
    }
}
