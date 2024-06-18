public abstract class ControllerBase
{
    protected Player player;
    public virtual void Init(Player player)
    {
        this.player = player;
    }

    public virtual void Update() { }
    public virtual void Enter() { }
    public virtual void Exit() { }
}
