using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public sealed class InputController : MonoBehaviour
{
    private InputReader _reader; //можем ли мы подменить приватное поле созданного нами типа?
    private bool _hadMovementInput;
    private bool _isGameplayInputBlocked;
    private bool _isFighter;
    private bool _blockInputActive;

    //private UnityAction _locomotionDirectionUndirectedHandler;
    public UnityAction LocomotionDirectionUndirected;
    public UnityAction AttackCloseRangeButtonPressed;
    public UnityAction AttackLongRangeButtonPressed;
    public UnityAction OpeningGameplayeMenuButtonPressed;
    public UnityAction TransformButtonPressed;
    public UnityAction BlockButtonHolded;
    public UnityAction BlockButtonUnholded;
    //private UnityAction<Vector2> _locomotionDirectionDirectedHandler;
    public UnityAction<Vector2> LocomotionDirectionDirected;

    public bool IsRunning => _reader.MainCharacter.Running.IsPressed();
    private bool GameplayInputEnabled => !_isGameplayInputBlocked && Time.timeScale > 0f;
    private bool CombatInputEnabled => GameplayInputEnabled && !_isFighter;

    private void Awake()
    {
        //_reader.MainCharacter.OpeningGameplayMenu.performed += context => OpeningGameplayeMenuButtonPressed.Invoke();
        _reader = new InputReader();
    }

    private void OnEnable()
    {
        var actions = _reader.MainCharacter;
        actions.AttackCloseRange.performed += OnAttackCloseRange;
        actions.AttackLongRange.performed += OnAttackLongRange;
        actions.Block.performed += OnBlockPerformed;
        actions.Block.canceled += OnBlockCanceled;
        actions.Transform.performed += OnTransform;
        actions.OpeningGameplayMenu.performed += OnOpeningGameplayMenu; //
        //_reader.MainCharacter.OpeningGameplayMenu.performed += context => OnOpeningGameplayeMenuButtonPressed(); //
        _reader.Enable();
    }

    private void OnDisable()
    {
        var actions = _reader.MainCharacter;
        actions.AttackCloseRange.performed -= OnAttackCloseRange;
        actions.AttackLongRange.performed -= OnAttackLongRange;
        actions.Block.performed -= OnBlockPerformed;
        actions.Block.canceled -= OnBlockCanceled;
        actions.Transform.performed -= OnTransform;
        actions.OpeningGameplayMenu.performed -= OnOpeningGameplayMenu; //
        //_reader.MainCharacter.OpeningGameplayMenu.performed -= context => OnOpeningGameplayeMenuButtonPressed(); //
        _reader.Disable();
        ReleaseBlockInput();
    }

    private void OnDestroy()
    {
        _reader?.Dispose();
    }

    private void Update()
    {
        if (!CombatInputEnabled)
        {
            ReleaseBlockInput();
        }

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
        if (isFighter)
        {
            ReleaseBlockInput();
        }
    }

    public void SetGameplayInputBlocked(bool isBlocked)
    {
        _isGameplayInputBlocked = isBlocked;
        if (_isGameplayInputBlocked)
        {
            ReleaseBlockInput();
        }
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

    private void OnBlockPerformed(InputAction.CallbackContext context)
    {
        if (CombatInputEnabled && !_blockInputActive)
        {
            _blockInputActive = true;
            BlockButtonHolded?.Invoke();
        }
    }

    private void OnBlockCanceled(InputAction.CallbackContext context)
    {
        ReleaseBlockInput();
    }

    private void ReleaseBlockInput()
    {
        if (_blockInputActive)
        {
            _blockInputActive = false;
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
