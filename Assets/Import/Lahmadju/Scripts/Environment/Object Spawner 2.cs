using UnityEngine;

public class ObjectsSpawner2 : MonoBehaviour
{
    [SerializeField] private GameObject spawningObject;
    [SerializeField] private GameObject spawningObjectRotationPivot;
    [SerializeField, Min(0.01f)] private float spawnCooldown = 5f;

    public void StartSpawning()
    {
        if (!isActiveAndEnabled || IsInvoking(nameof(SpawnWave)))
        {
            return;
        }

        SpawnWave();
        float interval = Mathf.Max(0.01f, spawnCooldown);
        InvokeRepeating(nameof(SpawnWave), interval, interval);
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(SpawnWave));
        CancelInvoke(nameof(ObjectCreation));
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    private void SpawnWave()
    {
        ObjectCreation();
        for (int i = 1; i < 4; i++)
        {
            Invoke(nameof(ObjectCreation), i / 1.5f);
        }
    }

    private void ObjectCreation()
    {
        GameObject ship = Instantiate(spawningObject, transform.position, spawningObjectRotationPivot.transform.rotation);
        ship.GetComponent<ConstantForce>().relativeForce = new Vector3(0, 0, -5000f);
        Destroy(ship, 10f);
    }
}
