using UnityEngine;

public class Player : MonoBehaviour, IAttacker, IDamageable //я бы еще накидал контрактов на управляемое перемещение
{
    private readonly PlayerController _controller = new PlayerController(); //можно использовать DI, но пока что это излишняя гибкость

    private void Update()
    {
        //LocomoteInUpdate(); //обрабатываться движение должно в монобехе PlayerMovementController, чую. Потому что когда появляется сигнал в PlayerInputController - он поднимается сюда, чтобы вызваться отсюда. И по идее все верно, так как Player реализует соответствующие интрфейсы, но хз правильно ли это
    }

    public void Initialize(Animator animator, float speed, Weapon weapon)
    {
        InitializeController(animator, speed, weapon);
    }

    public void Locomote(Vector2 direction)
    {
        _controller.Locomote(transform, direction);
    }

    public void Run(Vector2 direction)
    {
        _controller.Run(transform, direction);
    }

    public void Attack(IAttack attack, IDamageCalculator damageCalculator, Weapon weapon, Character damageReciever)
    {
        _controller.Attack(attack, damageCalculator, weapon, damageReciever);
    }

    public void ApplyDamage(float damage)
    {
        //_playerController.PlayerHealthController....
    }

    private void InitializeController(Animator animator, float speed, Weapon weapon)
    {
        _controller.Initialize(animator, speed, weapon);
    }

    private void CalibratePlayerRotation(Vector2 direction)
    {
        //transform.rotation = 
    }
}
