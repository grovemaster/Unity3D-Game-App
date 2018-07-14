using Data.Enum.Player;

namespace ECS.Component.Player
{
    public interface IPlayerComponent
    {
        PlayerColor PlayerColor { get; set; }
    }
}
