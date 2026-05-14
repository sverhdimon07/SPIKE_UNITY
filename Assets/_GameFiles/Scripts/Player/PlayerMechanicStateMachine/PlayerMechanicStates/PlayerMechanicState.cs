public abstract class PlayerMechanicState
{
    public abstract void Enter(Player player);

    public abstract void DoLogic(Player player);

    public abstract void DoLogicWithinFrame(Player player);

    public abstract void Exit(Player player);
}
