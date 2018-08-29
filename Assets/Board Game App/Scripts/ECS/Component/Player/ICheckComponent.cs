namespace ECS.Component.Player
{
    public interface ICheckComponent : IComponent
    {
        bool CommanderInCheck { get; set; }
    }
}
