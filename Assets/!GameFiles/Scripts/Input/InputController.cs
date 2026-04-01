using UnityEngine;
using UnityEngine.Events;

public sealed class InputController : MonoBehaviour
{
    private InputReader _reader; //можем ли мы подменить приватное поле созданного нами типа?

    private Vector2 _locomotionDirection;

    public UnityAction LocomotionDirectionUndirected;
    public UnityAction RunningButtonHolded;
    public UnityAction RunningButtonUnholded;
    public UnityAction AttackCloseRangeButtonPressed;
    public UnityAction AttackLongRangeButtonPressed;
    public UnityAction OpeningGameplayeMenuButtonPressed;

    public UnityAction<Vector2> LocomotionDirectionDirected;

    private void Update()
    {
        _locomotionDirection = _reader.MainCharacter.Locomotion.ReadValue<Vector2>();

        if (_locomotionDirection == Vector2.zero)
        {
            LocomotionDirectionUndirected.Invoke(); //тут бесконечно происходит вызов, мб добавить счетчик, НО некритично

            return;
        }
        LocomotionDirectionDirected.Invoke(_locomotionDirection);
    }

    private void OnEnable()
    {
        _reader.Enable();

        _reader.MainCharacter.Running.started += context => RunningButtonHolded.Invoke();
        _reader.MainCharacter.Running.canceled += context => RunningButtonUnholded.Invoke();
        _reader.MainCharacter.AttackCloseRange.performed += context => AttackCloseRangeButtonPressed.Invoke();
        _reader.MainCharacter.AttackLongRange.performed += context => AttackLongRangeButtonPressed.Invoke();
        _reader.MainCharacter.OpeningGameplayMenu.performed += context => OpeningGameplayeMenuButtonPressed.Invoke();
    }

    private void OnDisable()
    {
        _reader.MainCharacter.Running.started -= context => RunningButtonHolded.Invoke();
        _reader.MainCharacter.Running.canceled -= context => RunningButtonUnholded.Invoke();
        _reader.MainCharacter.AttackCloseRange.performed -= context => AttackCloseRangeButtonPressed.Invoke();
        _reader.MainCharacter.AttackLongRange.performed -= context => AttackLongRangeButtonPressed.Invoke();
        _reader.MainCharacter.OpeningGameplayMenu.performed -= context => OpeningGameplayeMenuButtonPressed.Invoke();

        _reader.Disable();
    }

    public void Initialize(InputReader reader)
    {
        _reader = reader;
    }
}
