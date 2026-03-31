using UnityEngine;
using UnityEngine.Events;

public sealed class InputController : MonoBehaviour //хз как без этого вызывать в Awake нужные мне функции из классов низкого уровня (чтобы они при этом не были монобехами). Проблема тут в том, что по-моему мы все равно можем подменить приватное поле созданного нами типа
{
    private InputReader _reader;

    private Vector2 _locomotionDirection;

    private bool isRunning;

    public UnityAction AttackCloseRangeButtonPressed;
    public UnityAction AttackLongRangeButtonPressed;
    public UnityAction OpeningGameplayeMenuButtonPressed;

    public UnityAction<Vector2> LocomotionDirectionDirected;
    //public UnityAction<Vector2> LocomotionDirectionUndirected;
    public UnityAction<Vector2> RunningButtonHolded;
    //public UnityAction<Vector2> RunningButtonUnholded;

    public Vector2 LocomotionDirection => _locomotionDirection;

    private void Update()
    {
        _locomotionDirection = _reader.MainCharacter.Locomotion.ReadValue<Vector2>();

        if (_locomotionDirection == Vector2.zero)
        {
            if (isRunning == true)
            {
                RunningButtonHolded.Invoke(_locomotionDirection);

                return;
            }
            //_locomotionDirectionUndirected.Invoke(_locomotionDirection);
            LocomotionDirectionDirected.Invoke(_locomotionDirection);

            return;
        }

        if (isRunning == true)
        {
            RunningButtonHolded.Invoke(_locomotionDirection);

            return;
        }

        LocomotionDirectionDirected.Invoke(_locomotionDirection);
    }

    private void OnEnable()
    {
        _reader.Enable();

        _reader.MainCharacter.Running.started += context => MakeIsRunningTrue();
        _reader.MainCharacter.Running.canceled += context => MakeIsRunningFalse();
        _reader.MainCharacter.AttackCloseRange.performed += context => AttackCloseRangeButtonPressed.Invoke();
        _reader.MainCharacter.AttackLongRange.performed += context => AttackLongRangeButtonPressed.Invoke();
        _reader.MainCharacter.OpeningGameplayMenu.performed += context => OpeningGameplayeMenuButtonPressed.Invoke();
    }

    private void OnDisable()
    {
        _reader.MainCharacter.Running.started -= context => MakeIsRunningTrue();
        _reader.MainCharacter.Running.canceled -= context => MakeIsRunningFalse();
        _reader.MainCharacter.AttackCloseRange.performed -= context => AttackCloseRangeButtonPressed.Invoke();
        _reader.MainCharacter.AttackLongRange.performed -= context => AttackLongRangeButtonPressed.Invoke();
        _reader.MainCharacter.OpeningGameplayMenu.performed -= context => OpeningGameplayeMenuButtonPressed.Invoke();

        _reader.Disable();
    }

    public void Initialize()
    {
        _reader = new InputReader();
    }

    private void MakeIsRunningTrue()
    {
        isRunning = true;
    }

    private void MakeIsRunningFalse()
    {
        isRunning = false;
    }
}
