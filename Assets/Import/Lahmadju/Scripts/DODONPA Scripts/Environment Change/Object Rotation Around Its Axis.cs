using System.Collections;
using UnityEngine;

enum Axises
{
    x,
    y,
    z
}

[RequireComponent(typeof(Transform))]
public class ObjectRotationAroundItsAxis : MonoBehaviour //добавить вращение ПО и ПРОТИВ ЧС
{
    [SerializeField] private Axises _axis;

    private Coroutine firstCoroutine;

    private Transform objectTransform;

    private readonly float timeBetweenChangingBarrelRotation = 0.005f;

    private readonly float currentSpeed = 0.2f;

    private void Awake()
    {
        objectTransform = GetComponent<Transform>();
    }

    private void Start() //В Entry Point!
    {
        StartAnimationRoutine();
    }

    public void StartAnimationRoutine()
    {
        firstCoroutine = StartCoroutine(Play());
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    private IEnumerator Play()
    {
        if (_axis == Axises.x)
        {
            while (true)
            {
                objectTransform.Rotate(-currentSpeed, 0f, 0f);

                yield return new WaitForSeconds(timeBetweenChangingBarrelRotation);
            }
        }
        else if (_axis == Axises.y)
        {
            while (true)
            {
                objectTransform.Rotate(0f, -currentSpeed, 0f);

                yield return new WaitForSeconds(timeBetweenChangingBarrelRotation);
            }
        }
        else
        {
            while (true)
            {
                objectTransform.Rotate(0f, 0f, -currentSpeed);

                yield return new WaitForSeconds(timeBetweenChangingBarrelRotation);
            }
        }
    }
}
