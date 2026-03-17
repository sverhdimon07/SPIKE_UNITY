using UnityEngine;
using UnityEngine.Events;

public abstract class EnvironmentAreaOverlapAnalyzer<T> : IEnvironmentAreaAnalyzer<T> where T : class //надо буквально на листочке расписать ту иерархию, которую я хочу И, вероятно, заменить тут интерфейс на АК, так как нам нужно очень много базовой реализации. НО нет кста, я хз, чо за реализация будет в рейкасте и коллайдерах, поэтому нужно юзать и интерфейсы, и АК
{
    protected Vector3 _startPosition;

    protected float _range;

    private readonly float _radius = 0.25f;

    public static UnityAction<Vector3, float> overlapCreated;

    public void Initialize(Vector3 actingEntityPosition, Vector2 actingEntityDirection, float range)
    {
        CalculateRequiredStartPosition(actingEntityPosition, actingEntityDirection);

        _range = range;
    }

    public virtual T Analyze(Vector3 actingEntityPosition, Vector2 actingEntityDirection)
    {
        CalculateRequiredStartPosition(actingEntityPosition, actingEntityDirection);

        Collider[] recievedColliders = Physics.OverlapSphere(_startPosition, _radius); //хз, есть ли разница в том, если создавать локальную переменную И если помещать полученные данные в поле; Возможно, получение коллайдеров стоит вынести в отдельный метод

        overlapCreated.Invoke(_startPosition, _radius);

        if (recievedColliders.Length == 0) //МГ
        {
            return null;
        }
        foreach (Collider collider in recievedColliders)
        {
            //Debug.Log(collider.name);
            if (collider.TryGetComponent(out T behavior) == false) //МГ
            {
                //Debug.Log("BBBBBBBBBBBBBBBBBBBB");
                return null;
            }
            //Debug.Log("AAAAAAAAAAAAAAAAA");
            return behavior;
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

    protected void CalculateRequiredStartPosition(Vector3 playerPosition, Vector2 playerDirection)
    {
        Vector3 requiredDirection = new Vector3(playerDirection.x, 0f, playerDirection.y).normalized / 2f; //МГ

        _startPosition = playerPosition + requiredDirection * _range + new Vector3(0f, 1.25f, 0f); //МГ
    }
}
