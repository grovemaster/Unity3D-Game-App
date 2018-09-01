using Data.Enums.Piece.Drop;
using Data.Enums.Piece.PostMove;
using System.Collections.Generic;

namespace Data.Piece
{
    public interface IAbilities
    {
        List<DropAbility> Drop { get; } // Piece has maximum of one drop ability
        List<PostMoveAbility> PostMove { get; } // Piece has maximum of one post-move ability
    }
}
