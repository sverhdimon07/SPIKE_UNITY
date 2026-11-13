using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoadEventWall : MonoBehaviour
{
    //private const string PLAYER_TAG = "PlayerBox";

    [SerializeField] private BoxCollider mainDoorsCollider;
    [SerializeField] private ObjectSpawner1 shipSpawner1;
    [SerializeField] private ObjectsSpawner2 shipSpawner2;
    [SerializeField] private ObjectSpawner3 shipSpawner3;

    [SerializeField] private UnityEvent wallActivated;
    private bool _shipSpawningStopped;

    private void OnTriggerEnter(Collider collider)
    {
        if (shipSpawner1 != null || shipSpawner2 != null || shipSpawner3 != null)
        {
            StartShipSpawning(collider);
            return;
        }

        if (!collider.TryGetComponent<PlayerController>(out _))
        {
            return;
        }

        wallActivated.Invoke();
        Invoke(nameof(DisableIsTriggerMainDoorsCollider), 1f);
    }

    private void StartShipSpawning(Collider collider)
    {
        if (_shipSpawningStopped || !collider.TryGetComponent<PlayerController>(out var player) || player.IsFlightLocked)
        {
            return;
        }

        shipSpawner1?.StartSpawning();
        shipSpawner2?.StartSpawning();
        shipSpawner3?.StartSpawning();
    }

    public void StopShipSpawning()
    {
        _shipSpawningStopped = true;
        shipSpawner1?.StopSpawning();
        shipSpawner2?.StopSpawning();
        shipSpawner3?.StopSpawning();
    }

    private void DisableIsTriggerMainDoorsCollider()
    {
        mainDoorsCollider.isTrigger = false;
    }
}
