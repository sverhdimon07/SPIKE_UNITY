using UnityEngine; //нужен ли MonoBehaviour?

public class PlayerRunning
{
    private float _speed;

    public void Initialize(float speed)
    {
        _speed = speed * speed;
    }

    public void Run(Transform point, Vector2 direction)
    {
        Vector3 directionCalibrated = new Vector3(direction.x, 0f, direction.y);

        point.position += directionCalibrated * _speed * Time.deltaTime;
    }
}
