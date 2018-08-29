using Data.Enum.Player;

namespace ECS.Component.Player
{
    public interface IPlayerComponent : IComponent
    {
        PlayerColor PlayerColor { get; set; }
    }
}
