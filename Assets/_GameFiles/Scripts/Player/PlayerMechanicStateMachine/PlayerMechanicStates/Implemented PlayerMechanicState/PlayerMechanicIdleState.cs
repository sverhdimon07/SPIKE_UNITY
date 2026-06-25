public sealed class PlayerMechanicIdleState : PlayerMechanicState
{
    public override void Enter(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override void Do(Player player, PlayerMechanicStateMachine stateMachine)
    {
        Player.Idled.Invoke(); //можно создать контроллер для айдла
    }

    public override void DoWithinFrame(Player player, PlayerMechanicStateMachine stateMachine)
    {
        //
    }

    public override bool TryExit(Player player, PlayerMechanicStateMachine stateMachine, PlayerMechanicState nextState)
    {
        stateMachine.SwitchState(player, nextState);

        return true;
    }
}
