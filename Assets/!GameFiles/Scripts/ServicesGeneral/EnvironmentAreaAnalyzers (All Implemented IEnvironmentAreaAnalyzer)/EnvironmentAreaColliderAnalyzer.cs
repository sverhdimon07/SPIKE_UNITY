using UnityEngine;

public class EnvironmentAreaColliderAnalyzer<T> : IEnvironmentAreaAnalyzer<T> where T : class
{
    //private SphereCollider _collider; //надо как-то делать инстанс геймобджекта с коллайдером или же создавать коллайдер внутри игрока, потом его удалять

    public void Initialize(Vector3 actingEntityPosition, Vector2 actingEntityDirection, float range)
    {
        //_collider = collider; //пока что мы не создаем инструмент здесь (я не придумал, как его не создавать каждый раз при атаке), я создал коллайдер на сцене и здесь его прокидываю
    }

    public T Analyze(Vector3 actingEntityPosition, Vector2 actingEntityDirection)
    {
        return null;
        /*
        if (_collider.TryGetComponent(out IDamageable damageable) == false)
        {
            return null;
        }
        return damageable;*/
    }
}
