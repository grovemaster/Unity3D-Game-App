using Data.Enum;
using System.Collections.Generic;

namespace Data.Piece
{
    public interface IPieceData
    {
        PieceType TypeOfPiece();
        // Return readonly data
        List<IMoveSet> Tiers(); // TODO Currently only implement 1st tier, other 2 tiers later
    }
}
