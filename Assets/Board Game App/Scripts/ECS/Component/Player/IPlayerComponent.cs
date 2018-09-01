using Data.Enums.Player;

namespace ECS.Component.Player
{
    public interface IPlayerComponent : IComponent
    {
        PlayerColor PlayerColor { get; set; }
    }
}
