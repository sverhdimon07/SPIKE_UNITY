using UnityEngine;
using UnityEngine.Events;

public class EnvironmentAreaOverlapAnalyzer : IEnvironmentAreaAnalyzer
{
    private Vector3 _startPosition;

    private float _raduis;

    public static UnityAction<Vector3, float> overlapCreated;

    public void Initialize(Vector3 playerPosition, Vector2 playerDirection, Vector3 weaponPrefabScale, float weaponAttackRange)
    {
        Vector3 requiredDirection = new Vector3(playerDirection.x, 0f, playerDirection.y).normalized; //МГ

        _startPosition = playerPosition + requiredDirection + new Vector3(0f, 1.25f, 0f); //МГ
        _raduis = weaponPrefabScale.y / 2.25f; //МГ
    }

    public IDamageable Analyze(Vector3 playerPosition, Vector2 playerDirection)
    {
        Vector3 requiredDirection = new Vector3(playerDirection.x, 0f, playerDirection.y).normalized; //МГ

        _startPosition = playerPosition + requiredDirection + new Vector3(0f, 1.25f, 0f); //МГ

        Collider[] recievedColliders = Physics.OverlapSphere(_startPosition, _raduis); //хз, есть ли разница в том, если создавать локальную переменную И если помещать полученные данные в поле; Возможно, получение коллайдеров стоит вынести в отдельный метод

        overlapCreated.Invoke(_startPosition, _raduis);

        if (recievedColliders.Length == 0) //МГ
        {
            return null;
        }
        foreach (Collider collider in recievedColliders)
        {
            //Debug.Log(collider.name);
            if (collider.TryGetComponent(out IDamageable damageable) == false) //МГ
            {
                //Debug.Log("BBBBBBBBBBBBBBBBBBBB");
                return null;
            }
            //Debug.Log("AAAAAAAAAAAAAAAAA");
            return damageable;
        }
        return null;
        /*
        if (recievedColliders[0].TryGetComponent(out IDamageable damageable) == false) //МГ
        {
            Debug.Log("BBBBBBBBBBBBBBBBBBBB");
            return null;
        }
        Debug.Log("AAAAAAAAAAAAAAAAA");
        return damageable;*/
    }
}
