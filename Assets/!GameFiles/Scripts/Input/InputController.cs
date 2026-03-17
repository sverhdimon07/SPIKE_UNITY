using UnityEngine;
using UnityEngine.Events;

public class InputController : MonoBehaviour //хз как без этого вызывать в Awake нужные мне функции из классов низкого уровн€ (чтобы они при этом не были монобехами). ѕроблема тут в том, что по-моему мы все равно можем подменить приватное поле созданного нами типа
{
    private InputReader _reader;

    private Vector2 _playerLocomotionDirection;
    private Vector2 _playerDirection = new Vector2(0f, 1f);

    public UnityAction<Vector2> AttackCloseRangeButtonPressed;
    public UnityAction<Vector2> AttackLongRangeButtonPressed;

    public UnityAction<Vector2> LocomotionDirectionDirected;
    //public UnityAction<Vector2> LocomotionDirectionUndirected;
    public UnityAction<Vector2> RunningButtonHolded;
    //public UnityAction<Vector2> RunningButtonUnholded;

    private bool isRunning;
    private bool isAttackingCloseRange;
    private bool isAttackingLongRange;

    public Vector2 PlayerLocomotionDirection => _playerLocomotionDirection;
    public Vector2 PlayerDirection => _playerDirection;

    private void Update()
    {
        if (isAttackingCloseRange == true)
        {
            AttackCloseRangeButtonPressed.Invoke(_playerDirection);

            MakeIsAttackingCloseRangeFalse();

            return;
        }
        else if (isAttackingLongRange == true)
        {
            AttackLongRangeButtonPressed.Invoke(_playerDirection);

            MakeIsAttackingLongRangeFalse();

            return;
        }
        //
        _playerLocomotionDirection = _reader.MainCharacter.Locomotion.ReadValue<Vector2>();

        if (_playerLocomotionDirection == Vector2.zero)
        {
            if (isRunning == true)
            {
                RunningButtonHolded.Invoke(_playerLocomotionDirection);

                return;
            }
            //_locomotionDirectionUndirected.Invoke(_locomotionDirection);
            LocomotionDirectionDirected.Invoke(_playerLocomotionDirection);

            return;
        }
        _playerDirection = _playerLocomotionDirection;

        if (isRunning == true)
        {
            RunningButtonHolded.Invoke(_playerLocomotionDirection);

            return;
        }

        LocomotionDirectionDirected.Invoke(_playerLocomotionDirection);
    }

    private void OnEnable()
    {
        Initialize(); //инит внутреннего содержимого в себе же - пока делаем здесь, а не в GameController

        _reader.MainCharacter.Running.started += context => MakeIsRunningTrue();
        _reader.MainCharacter.Running.canceled += context => MakeIsRunningFalse();
        _reader.MainCharacter.AttackCloseRange.performed += context => MakeIsAttackingCloseRangeTrue();
        _reader.MainCharacter.AttackLongRange.performed += context => MakeIsAttackingLongRangeTrue();
    }

    private void OnDisable()
    {
        _reader.MainCharacter.Running.started -= context => MakeIsRunningTrue();
        _reader.MainCharacter.Running.canceled -= context => MakeIsRunningFalse();
        _reader.MainCharacter.AttackCloseRange.performed -= context => MakeIsAttackingCloseRangeTrue();
        _reader.MainCharacter.AttackLongRange.performed -= context => MakeIsAttackingLongRangeTrue();

        _reader.Disable();
    }

    public void Initialize()
    {
        _reader = new InputReader();

        _reader.Enable();
    }

    private void MakeIsRunningTrue()
    {
        isRunning = true;
    }

    private void MakeIsRunningFalse()
    {
        isRunning = false;
    }

    private void MakeIsAttackingCloseRangeTrue()
    {
        isAttackingCloseRange = true;
    }

    private void MakeIsAttackingCloseRangeFalse()
    {
        isAttackingCloseRange = false;
    }

    private void MakeIsAttackingLongRangeTrue()
    {
        isAttackingLongRange = true;
    }

    private void MakeIsAttackingLongRangeFalse()
    {
        isAttackingLongRange = false;
    }
}
