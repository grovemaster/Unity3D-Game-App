using Data.Enum;
using System.Collections.Generic;

namespace Data.Piece
{
    public interface IPieceData
    {
        PieceType TypeOfPiece { get; }
        IAbilities Abilities { get; }
        // Return readonly data, maybe by turning this into a property?
        List<IMoveSet> Tiers { get; } // TODO Currently only implement 1st tier, other 2 tiers later
    }
}
