using UnityEngine; //нужен ли MonoBehaviour?
using UnityEngine.Events;

public class InputController : MonoBehaviour //хз как без этого вызывать в Awake нужные мне функции из классов низкого уровн€ (чтобы они при этом не были монобехами). ѕроблема тут в том, что по-моему мы все равно можем подменить приватное поле созданного нами типа
{
    private InputReader _reader;

    private Vector2 _locomotionDirection;

    public UnityAction<IAttack, IDamageCalculator, Weapon, Character> _attackCloseRangeButtonPressed;
    public UnityAction<IAttack, IDamageCalculator, Weapon, Character> _attackLongRangeButtonPressed;

    public UnityAction<Vector2> _locomotionDirectionDirected;
    //public UnityAction<Vector2> _locomotionDirectionUndirected;
    public UnityAction<Vector2> _runningButtonHolded;
    //public UnityAction<Vector2> _runningButtonUnholded;

    private bool isRunning;
    private bool isAttackingCloseRange;
    private bool isAttackingLongRange;

    private void Update()
    {
        if (isAttackingCloseRange == true)
        {
            _attackCloseRangeButtonPressed.Invoke(new PlayerAttackCloseRange(), new DamageCalculatorBasic(), FindAnyObjectByType<Weapon>(), FindAnyObjectByType<Character>()); //оч плохо, передача зависимости должна быть в GameCOntroller

            MakeIsAttackingCloseRangeFalse();

            return;
        }
        else if (isAttackingLongRange == true)
        {
            _attackLongRangeButtonPressed.Invoke(new PlayerAttackLongRange(), new DamageCalculatorBasic(), FindAnyObjectByType<Weapon>(), FindAnyObjectByType<Character>()); //оружий может быть много на уровне (их как миниму 2 - у игрока и у персонажа)

            MakeIsAttackingLongRangeFalse();

            return;
        }
        //
        _locomotionDirection = _reader.MainCharacter.Locomotion.ReadValue<Vector2>();

        if (_locomotionDirection == Vector2.zero)
        {
            if (isRunning == true)
            {
                _runningButtonHolded.Invoke(_locomotionDirection);

                return;
            }
            //_locomotionDirectionUndirected.Invoke(_locomotionDirection);
            _locomotionDirectionDirected.Invoke(_locomotionDirection);

            return;
        }
        if (isRunning == true)
        {
            _runningButtonHolded.Invoke(_locomotionDirection);

            return;
        }

        _locomotionDirectionDirected.Invoke(_locomotionDirection);
    }

    private void OnEnable()
    {
        InitializeReader(); //инит внутреннего содержимого в себе же - ’«
        EnableReader();

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

        DisableReader();
    }

    public void Initialize()
    {
        //InitializeReader();
    }

    private void InitializeReader()
    {
        _reader = new InputReader();
    }

    private void EnableReader()
    {
        _reader.Enable();
    }

    private void DisableReader()
    {
        _reader.Disable();
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
