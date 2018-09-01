using Data.Enums.Piece.Drop;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using System.Collections.Generic;

namespace Data.Piece
{
    public interface IAbilities
    {
        List<PreMoveAbility> PreMove { get; }
        List<DropAbility> Drop { get; }
        List<PostMoveAbility> PostMove { get; }
    }
}
