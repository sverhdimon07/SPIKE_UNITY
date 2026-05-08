using UnityEngine;

public class CharacterLocomotion 
{
    private float _speed;

    public void Initialize(float speed)
    {
        _speed = speed;
    }

    public void LocomoteWithinFrame(Transform characterPoint, Vector2 locomotionDirection)
    {
        Vector3 directionCalibrated = new Vector3(locomotionDirection.x, 0f, locomotionDirection.y);

        characterPoint.position += directionCalibrated * _speed * Time.deltaTime;
    }
}
