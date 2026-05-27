//using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAI : MonoBehaviour
{
    //[SerializeField] private CharacterAnimator characterAnimator;

    //[SerializeField] private AIDestinationSetter destinationSetter;
    //[SerializeField] private AILerp aiLerp;

    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform rootMotionFixTransform;

    [SerializeField] private Transform roamTarget;

    [SerializeField] private Transform point1;
    [SerializeField] private Transform point_1;
    [SerializeField] private Transform point2;
    [SerializeField] private Transform point3;
    [SerializeField] private Transform point_3;
    [SerializeField] private Transform point4;
    [SerializeField] private Transform point5;
    [SerializeField] private Transform point_5;
    [SerializeField] private Transform point6;
    [SerializeField] private Transform point7;
    [SerializeField] private Transform point8;

    private Vector3 roamPosition;

    private bool roamingState = false;

    [SerializeField] private UnityEvent OnAIEnabled;
    [SerializeField] private UnityEvent OnPoint3Destination;

    private int counter = 0;

    private void Start()
    {
        roamingState = true;

        roamPosition = point1.position;

        //characterAnimator.ControlWalkingAnimation(true);
    }
    private void Update()
    {
        if (roamingState == true)
        {
            RoamingStateLogic();
        }
    }
    private void RoamingStateLogic()
    {
        roamTarget.position = roamPosition;
        if (Vector3.Distance(gameObject.transform.position, roamPosition) <= 0.4f)
        {
            if (roamPosition == point1.position)
            {
                roamPosition = point_1.position;
            }
            if (roamPosition == point_1.position)
            {
                roamPosition = point2.position;
            }
            else if (roamPosition == point2.position)
            {
                roamPosition = point3.position;
            }
            else if (roamPosition == point3.position)
            {
                roamPosition = point_3.position;
            }
            else if (roamPosition == point_3.position)
            {
                roamPosition = point4.position;
            }
            else if (roamPosition == point4.position)
            {
                roamPosition = point5.position;
            }
            else if (roamPosition == point5.position)
            {
                roamPosition = point_5.position;
            }
            else if (roamPosition == point_5.position)
            {
                roamPosition = point6.position;
            }
            else if (roamPosition == point6.position)
            {
                if (counter < 1)
                {
                    DisableAI();
                    OnPoint3Destination?.Invoke();

                    counter += 1;
                }
                roamPosition = point7.position;
            }
            else if (roamPosition == point7.position)
            {
                roamPosition = point8.position;
            }
        }
        //destinationSetter.target = roamTarget;
    }

    public void TurnCharacter()
    {
        characterTransform.rotation *= Quaternion.Euler(0, 180, 0);
    }

    public void IncreaseSpeed()
    {
        //aiLerp.speed = 7f;
    }

    public void DecreaseSpeed()
    {
        //aiLerp.speed = 4f;
    }

    public void EnableAI()
    {
        //characterTransform.position = rootMotionFixTransform.position;

        //aiLerp.enabled = true;

        OnAIEnabled?.Invoke();
    }

    public void DisableAI()
    {
        //aiLerp.enabled = false;
    }
}
