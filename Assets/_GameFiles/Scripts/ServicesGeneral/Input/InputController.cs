using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public sealed class InputController : MonoBehaviour
{
    private InputReader _reader;
    private bool _hadMovementInput;
    private bool _isGameplayMenuOpened;
    private bool _isFighter;

    public UnityAction LocomotionDirectionUndirected;
    public UnityAction AttackCloseRangeButtonPressed;
    public UnityAction AttackLongRangeButtonPressed;
    public UnityAction OpeningGameplayeMenuButtonPressed;
    public UnityAction TransformButtonPressed;
    public UnityAction BlockButtonHolded;
    public UnityAction BlockButtonUnholded;
    public UnityAction<Vector2> LocomotionDirectionDirected;

    public bool IsRunning => _reader.MainCharacter.Running.IsPressed();
    private bool GameplayInputEnabled => !_isGameplayMenuOpened && Time.timeScale > 0f;
    private bool CombatInputEnabled => GameplayInputEnabled && !_isFighter;

    private void Awake()
    {
        _reader = new InputReader();
    }

    private void OnEnable()
    {
        var actions = _reader.MainCharacter;
        actions.AttackCloseRange.performed += OnAttackCloseRange;
        actions.AttackLongRange.performed += OnAttackLongRange;
        actions.Block.started += OnBlockStarted;
        actions.Block.canceled += OnBlockCanceled;
        actions.Transform.performed += OnTransform;
        actions.OpeningGameplayMenu.performed += OnOpeningGameplayMenu;
        _reader.Enable();
    }

    private void OnDisable()
    {
        var actions = _reader.MainCharacter;
        actions.AttackCloseRange.performed -= OnAttackCloseRange;
        actions.AttackLongRange.performed -= OnAttackLongRange;
        actions.Block.started -= OnBlockStarted;
        actions.Block.canceled -= OnBlockCanceled;
        actions.Transform.performed -= OnTransform;
        actions.OpeningGameplayMenu.performed -= OnOpeningGameplayMenu;
        _reader.Disable();
    }

    private void OnDestroy()
    {
        _reader?.Dispose();
    }

    private void Update()
    {
        if (!GameplayInputEnabled)
        {
            return;
        }

        Vector2 direction = _reader.MainCharacter.Locomotion.ReadValue<Vector2>();
        bool hasMovementInput = direction != Vector2.zero;
        bool stopped = _hadMovementInput && !hasMovementInput;
        _hadMovementInput = hasMovementInput;

        if (hasMovementInput)
        {
            LocomotionDirectionDirected?.Invoke(direction);
        }
        else if (stopped)
        {
            LocomotionDirectionUndirected?.Invoke();
        }
    }

    public void SetFighterForm(bool isFighter)
    {
        _isFighter = isFighter;
    }

    public void OnOpeningGameplayMenuButtonPressed()
    {
        _isGameplayMenuOpened = !_isGameplayMenuOpened;
    }

    private void OnAttackCloseRange(InputAction.CallbackContext context)
    {
        if (CombatInputEnabled)
        {
            AttackCloseRangeButtonPressed?.Invoke();
        }
    }

    private void OnAttackLongRange(InputAction.CallbackContext context)
    {
        if (CombatInputEnabled)
        {
            AttackLongRangeButtonPressed?.Invoke();
        }
    }

    private void OnBlockStarted(InputAction.CallbackContext context)
    {
        if (CombatInputEnabled)
        {
            BlockButtonHolded?.Invoke();
        }
    }

    private void OnBlockCanceled(InputAction.CallbackContext context)
    {
        if (!_isFighter)
        {
            BlockButtonUnholded?.Invoke();
        }
    }

    private void OnTransform(InputAction.CallbackContext context)
    {
        if (GameplayInputEnabled)
        {
            TransformButtonPressed?.Invoke();
        }
    }

    private void OnOpeningGameplayMenu(InputAction.CallbackContext context)
    {
        OpeningGameplayeMenuButtonPressed?.Invoke();
    }
}
