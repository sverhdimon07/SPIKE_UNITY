public abstract class PlayerMechanicState
{
    public abstract void Enter(Player player);

    public abstract void Do(Player player);

    public abstract void DoWithinFrame(Player player);

    public abstract void Exit(Player player);
}
