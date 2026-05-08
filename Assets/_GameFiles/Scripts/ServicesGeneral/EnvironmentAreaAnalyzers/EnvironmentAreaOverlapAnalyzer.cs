using UnityEngine;
using UnityEngine.Events;

public sealed/*abstract*/ class EnvironmentAreaOverlapAnalyzer<RequiredInterface, ExceptionType> : IEnvironmentAreaAnalyzer<RequiredInterface, ExceptionType> where RequiredInterface : class where ExceptionType : class //надо буквально на листочке расписать ту иерархию, которую я хочу И, вероятно, заменить тут интерфейс на АК, так как нам нужно очень много базовой реализации. НО нет кста, я хз, чо за реализация будет в рейкасте и коллайдерах, поэтому нужно юзать и интерфейсы, и АК
{
    private Vector3 _startSphereCenterPosition;

    private float _sphereCenterDistanceToActingEntity;
    private float _sphereRadius;
    private float _sphereHeight;

    public static UnityAction<Vector3, float> OverlapCreated; //могу это переделать под нестатическую логику как в событиях InputController, но пока удобнее сделать быстро и статически

    public void Initialize(Vector3 actingEntityPosition, Vector2 actingEntityDirection, float sphereDistanceToActingEntity, float sphereRadius, float sphereHeight)
    {
        _sphereCenterDistanceToActingEntity = sphereDistanceToActingEntity + sphereRadius;
        _sphereRadius = sphereRadius;
        _sphereHeight = sphereHeight;

        CalculateRequiredStartSphereCenterPosition(actingEntityPosition, actingEntityDirection);
    }

    public /*virtual*/ RequiredInterface Analyze(Vector3 actingEntityPosition, Vector2 actingEntityDirection)
    {
        CalculateRequiredStartSphereCenterPosition(actingEntityPosition, actingEntityDirection);

        Collider[] recievedColliders = Physics.OverlapSphere(_startSphereCenterPosition, _sphereRadius); //хз, есть ли разница в том, если создавать локальную переменную И если помещать полученные данные в поле; Возможно, получение коллайдеров стоит вынести в отдельный метод

        OverlapCreated.Invoke(_startSphereCenterPosition, _sphereRadius);

        if (recievedColliders == null)
        {
            return null;
        }
        foreach (Collider collider in recievedColliders)
        {
            if (collider.TryGetComponent(out ExceptionType entity) == true)
            {
                continue;
            }
            if (collider.TryGetComponent(out RequiredInterface behavior) == false)
            {
                continue;
            }
            return behavior;
        }
        return null;
    }

    private void CalculateRequiredStartSphereCenterPosition(Vector3 playerPosition, Vector2 playerDirection)
    {
        Vector3 requiredDirection = new Vector3(playerDirection.x, 0f, playerDirection.y).normalized; //Хз, как правильно назвать локальную переменную;
        Vector3 requiredStartSphereCenterPosition = playerPosition + requiredDirection * _sphereCenterDistanceToActingEntity + new Vector3(0f, + _sphereHeight, 0f); //+ new Vector3(0f, _sphereHeight, _sphereCenterDistanceToActingEntity);

        _startSphereCenterPosition = requiredStartSphereCenterPosition;
    }
}
