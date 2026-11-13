using UnityEngine;

public sealed class FlightLimiter : MonoBehaviour
{
    [SerializeField] private CharacterFactory _closeRangeSpawner;
    [SerializeField] private CharacterFactory _longRangeSpawner;
    [SerializeField] private RoadEventWall _shipSpawnTrigger;

    private CharacterControllerNew _closeRangeEnemy;
    private CharacterControllerNew _longRangeEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            Reach(player);
        }
    }

    public void Reach(PlayerController player)
    {
        if (!isActiveAndEnabled || player.IsFlightLocked)
        {
            return;
        }

        player.LockFighterForm();
        if (_shipSpawnTrigger != null)
        {
            _shipSpawnTrigger.StopShipSpawning();
        }

        RestoreEnemies(true, true);
    }

    public void RestoreEnemies(bool closeRange, bool longRange)
    {
        RestoreEnemy(ref _closeRangeEnemy, _closeRangeSpawner, closeRange);
        RestoreEnemy(ref _longRangeEnemy, _longRangeSpawner, longRange);
    }

    public bool IsAtOrPastBoundary(Vector3 levelStart, Vector3 position, CapsuleCollider playerCapsule)
    {
        BoxCollider boundary = GetComponent<BoxCollider>();
        Vector3 center = transform.TransformPoint(boundary.center);
        Vector3 normal = transform.right;
        float startSide = Mathf.Sign(Vector3.Dot(levelStart - center, normal));
        float distance = startSide * Vector3.Dot(position - center, normal);
        float halfWidth = 0.5f * boundary.size.x * Mathf.Abs(transform.lossyScale.x);
        Vector3 playerScale = playerCapsule.transform.lossyScale;
        float playerRadius = playerCapsule.radius * Mathf.Max(Mathf.Abs(playerScale.x), Mathf.Abs(playerScale.z));
        return distance <= halfWidth + playerRadius;
    }

    private void RestoreEnemy(ref CharacterControllerNew enemy, CharacterFactory spawner, bool shouldBePresent)
    {
        if (shouldBePresent && enemy == null && spawner != null)
        {
            enemy = spawner.SpawnAt(0);
        }
        else if (!shouldBePresent && enemy != null)
        {
            enemy.gameObject.SetActive(false);
            Destroy(enemy.gameObject);
            enemy = null;
        }
    }
}
