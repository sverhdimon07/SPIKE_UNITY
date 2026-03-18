using UnityEngine;

public class CharacterLocomotion 
{
    private float _speed;

    public void Initialize(float speed)
    {
        _speed = speed;
    }

    public void Locomote(Transform characterPoint, Transform characterRenderAndSkeletonPoint, Vector2 locomotionDirection)
    {
        /*
        //ROTATION
        if (locomotionDirection == Vector2.zero)
        {
            return;
        }

        Vector3 requiredDirection = new Vector3(locomotionDirection.x, 0f, locomotionDirection.y).normalized; //Ã√

        //Quaternion targetRotation = transform.rotation = Quaternion.LookRotation(requiredDirection);

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime); //Ã√


        characterRenderAndSkeletonPoint.rotation = Quaternion.LookRotation(requiredDirection);
        */

        //LOCOMOTION
        Vector3 directionCalibrated = new Vector3(locomotionDirection.x, 0f, locomotionDirection.y);

        characterPoint.position += directionCalibrated * _speed * Time.deltaTime;
    }
}
