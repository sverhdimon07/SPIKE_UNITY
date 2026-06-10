public sealed class PlayerMechanicIdleState : PlayerMechanicState
{
    public override void Enter(Player player)
    {
        //
    }

    public override void Do(Player player)
    {
        Player.Idled.Invoke(); //можно создать контроллер для айдла
    }

    public override void DoWithinFrame(Player player)
    {
        //
    }

    public override void Exit(Player player)
    {
        //player.MechanicStateMachine.SwitchState(player, new PlayerMechanicLocomotionState());
    }
}
