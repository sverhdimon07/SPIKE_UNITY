using UnityEngine;

public sealed class PlayerRunning
{
    private float _speed;

    public void Initialize(float speed)
    {
        _speed = speed;
    }

    public void RunWithinFrame(Transform playerPoint, Transform playerRenderAndSkeletonPoint, Vector2 locomotionDirection, Transform cameraPoint) //ВЫНЕСТИ НУЖНЫЕ БЛОКИ КОДА В ОТДЕЛЬНЫЕ МЕТОДЫ ДЛЯ ЧИТАЕМОСТИ КОДА И ЕГО ЧИСТОТЫ
    {
        if (locomotionDirection == Vector2.zero) //возможно перенести на уровень инпут контроллера
        {
            return;
        }

        //RUNNING
        Vector3 cameraForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized;
        Vector3 cameraRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized;
        Vector3 requiredDirection = (cameraForward * locomotionDirection.y + cameraRight * locomotionDirection.x).normalized;

        playerPoint.position += requiredDirection * _speed * Time.deltaTime;

        //ROTATION
        playerRenderAndSkeletonPoint.rotation = Quaternion.LookRotation(requiredDirection); //СДЕЛАТЬ ПОВОРОТЫ ПЛАВНЫМИ
    }
}
