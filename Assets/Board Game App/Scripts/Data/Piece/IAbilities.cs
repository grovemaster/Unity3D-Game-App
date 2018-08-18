using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;

namespace Data.Piece
{
    public interface IAbilities
    {
        DropAbility? Drop { get; } // Piece has maximum of one drop ability
        PostMoveAbility? PostMove { get; } // Piece has maximum of one post-move ability
    }
}
