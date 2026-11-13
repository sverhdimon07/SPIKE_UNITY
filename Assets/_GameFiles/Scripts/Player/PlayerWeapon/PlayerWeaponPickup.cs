using UnityEngine;

public sealed class PlayerWeaponPickup : MonoBehaviour
{
    [SerializeField] private Transform _weaponToEquip;
    [SerializeField] private Transform _weaponToUnequip;

    private Collider _restoredPlayerCollider;

    public void IgnorePlayerUntilExitIfOverlapping(Collider playerCollider)
    {
        if (TryGetComponent(out SphereCollider pickupCollider) &&
            Physics.ComputePenetration(
                pickupCollider, pickupCollider.transform.position, pickupCollider.transform.rotation,
                playerCollider, playerCollider.transform.position, playerCollider.transform.rotation,
                out _, out _))
        {
            _restoredPlayerCollider = playerCollider;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == _restoredPlayerCollider ||
            !other.TryGetComponent(out PlayerController player) ||
            _weaponToEquip == null || _weaponToUnequip == null ||
            !_weaponToEquip.IsChildOf(player.transform) ||
            !_weaponToUnequip.IsChildOf(player.transform))
        {
            return;
        }

        _weaponToUnequip.gameObject.SetActive(false);
        _weaponToEquip.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == _restoredPlayerCollider)
        {
            _restoredPlayerCollider = null;
        }
    }
}
