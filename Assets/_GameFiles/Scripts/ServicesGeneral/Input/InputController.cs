using System;
using UnityEngine;
using UnityEngine.Events;

public sealed class InputController : MonoBehaviour
{
    private InputReader _reader; //можем ли мы подменить приватное поле созданного нами типа?

    private Vector2 _locomotionDirection;

    private int _locomotionDirectionUndirectedCounter;

    private bool _isGameplayMenuOpened = false;

    public UnityAction LocomotionDirectionUndirected;
    public UnityAction RunningButtonHolded;
    public UnityAction RunningButtonUnholded;
    public UnityAction AttackCloseRangeButtonPressed;
    public UnityAction AttackLongRangeButtonPressed;
    public UnityAction OpeningGameplayeMenuButtonPressed;

    public UnityAction BlockButtonHolded;
    public UnityAction BlockButtonUnholded;

    public UnityAction<Vector2> LocomotionDirectionDirected;

    //private UnityAction _locomotionDirectionUndirectedHandler;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _runningButtonHoldedHandler;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _runningButtonUnholdedHandler;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _attackCloseRangeButtonPressedHandler;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _attackLongRangeButtonPressedHandler;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _blockButtonHoldedHandler;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _blockButtonUnholdedHandler;

    //private UnityAction<Vector2> _locomotionDirectionDirectedHandler;

    private void Awake()
    {
        _runningButtonHoldedHandler = delegate { RunningButtonHolded.Invoke(); };
        _runningButtonUnholdedHandler = delegate { RunningButtonUnholded.Invoke(); };
        _attackCloseRangeButtonPressedHandler = delegate { AttackCloseRangeButtonPressed.Invoke(); };
        _attackLongRangeButtonPressedHandler = delegate { AttackLongRangeButtonPressed.Invoke(); ; };
        //_reader.MainCharacter.OpeningGameplayMenu.performed += context => OpeningGameplayeMenuButtonPressed.Invoke();
        _blockButtonHoldedHandler = delegate { BlockButtonHolded.Invoke(); };
        _blockButtonUnholdedHandler = delegate { BlockButtonUnholded.Invoke(); };

        _reader = new InputReader();
    }

    private void OnEnable()
    {
        _reader.Enable();

        SubscribeAllInputsExceptOpeningGameplayMenu();

        _reader.MainCharacter.OpeningGameplayMenu.performed += context => OpeningGameplayeMenuButtonPressed.Invoke(); //
        //_reader.MainCharacter.OpeningGameplayMenu.performed += context => OnOpeningGameplayeMenuButtonPressed(); //
    }

    private void OnDisable()
    {
        UnsubscribeAllInputsExceptOpeningGameplayMenu();

        _reader.MainCharacter.OpeningGameplayMenu.performed -= context => OpeningGameplayeMenuButtonPressed.Invoke(); //
        //_reader.MainCharacter.OpeningGameplayMenu.performed -= context => OnOpeningGameplayeMenuButtonPressed(); //

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

    private void SubscribeAllInputsExceptOpeningGameplayMenu()
    {
        _reader.MainCharacter.Running.started += _runningButtonHoldedHandler;
        _reader.MainCharacter.Running.canceled += _runningButtonUnholdedHandler;
        _reader.MainCharacter.AttackCloseRange.performed += _attackCloseRangeButtonPressedHandler;
        _reader.MainCharacter.AttackLongRange.performed += _attackLongRangeButtonPressedHandler;
        _reader.MainCharacter.Block.started += _blockButtonHoldedHandler;
        _reader.MainCharacter.Block.canceled += _blockButtonUnholdedHandler;
    }

    private void UnsubscribeAllInputsExceptOpeningGameplayMenu()
    {
        _reader.MainCharacter.Running.started -= _runningButtonHoldedHandler;
        _reader.MainCharacter.Running.canceled -= _runningButtonUnholdedHandler;
        _reader.MainCharacter.AttackCloseRange.performed -= _attackCloseRangeButtonPressedHandler;
        _reader.MainCharacter.AttackLongRange.performed -= _attackLongRangeButtonPressedHandler;
        _reader.MainCharacter.Block.started -= _blockButtonHoldedHandler;
        _reader.MainCharacter.Block.canceled -= _blockButtonUnholdedHandler;
    }

    public void OnOpeningGameplayMenuButtonPressed()
    {
        if (_isGameplayMenuOpened == false)
        {
            _isGameplayMenuOpened = true;

            UnsubscribeAllInputsExceptOpeningGameplayMenu();
        }
        else if (_isGameplayMenuOpened == true)
        {
            _isGameplayMenuOpened = false;

            SubscribeAllInputsExceptOpeningGameplayMenu();
        }
    }
}
