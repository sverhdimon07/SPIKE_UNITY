using UnityEngine;
using UnityEngine.Events;

public sealed class InputController : MonoBehaviour
{
    private InputReader _reader; //можем ли мы подменить приватное поле созданного нами типа?

    private Vector2 _locomotionDirection;

    private int _locomotionDirectionUndirectedCounter;

    public UnityAction LocomotionDirectionUndirected;
    public UnityAction RunningButtonHolded;
    public UnityAction RunningButtonUnholded;
    public UnityAction AttackCloseRangeButtonPressed;
    public UnityAction AttackLongRangeButtonPressed;
    public UnityAction OpeningGameplayeMenuButtonPressed;

    public UnityAction BlockButtonHolded;
    public UnityAction BlockButtonUnholded;

    public UnityAction<Vector2> LocomotionDirectionDirected;

    private void Awake()
    {
        _reader = new InputReader();
    }

    private void OnEnable()
    {
        _reader.Enable();

        _reader.MainCharacter.Running.started += context => RunningButtonHolded.Invoke();
        _reader.MainCharacter.Running.canceled += context => RunningButtonUnholded.Invoke();
        _reader.MainCharacter.AttackCloseRange.performed += context => AttackCloseRangeButtonPressed.Invoke();
        _reader.MainCharacter.AttackLongRange.performed += context => AttackLongRangeButtonPressed.Invoke();
        _reader.MainCharacter.OpeningGameplayMenu.performed += context => OpeningGameplayeMenuButtonPressed.Invoke();

        _reader.MainCharacter.Block.started += context => BlockButtonHolded.Invoke();
        _reader.MainCharacter.Block.canceled += context => BlockButtonUnholded.Invoke();
    }

    private void OnDisable()
    {
        _reader.MainCharacter.Running.started -= context => RunningButtonHolded.Invoke();
        _reader.MainCharacter.Running.canceled -= context => RunningButtonUnholded.Invoke();
        _reader.MainCharacter.AttackCloseRange.performed -= context => AttackCloseRangeButtonPressed.Invoke();
        _reader.MainCharacter.AttackLongRange.performed -= context => AttackLongRangeButtonPressed.Invoke();
        _reader.MainCharacter.OpeningGameplayMenu.performed -= context => OpeningGameplayeMenuButtonPressed.Invoke();

        _reader.MainCharacter.Block.started -= context => BlockButtonHolded.Invoke();
        _reader.MainCharacter.Block.canceled -= context => BlockButtonUnholded.Invoke();

        _reader.Disable();
    }

    private void Update()
    {
        _locomotionDirection = _reader.MainCharacter.Locomotion.ReadValue<Vector2>();

        if (_locomotionDirection == Vector2.zero)
        {
            if (_locomotionDirectionUndirectedCounter < 1)
            {
                _locomotionDirectionUndirectedCounter += 1;

                LocomotionDirectionUndirected.Invoke();
            }
            return;
        }
        _locomotionDirectionUndirectedCounter = 0;

        LocomotionDirectionDirected.Invoke(_locomotionDirection);
    }
}
