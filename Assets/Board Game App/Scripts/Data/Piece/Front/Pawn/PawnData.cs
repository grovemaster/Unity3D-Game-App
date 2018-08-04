using System.Collections.Generic;
using Data.Enum;

namespace Data.Piece.Front.Pawn
{
    class PawnData : IPieceData
    {
        private static readonly List<IMoveSet> tiers;

        static PawnData()
        {
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new PawnMoveSetTier1st(), new PawnMoveSetTier2nd(), new PawnMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece()
        {
            return PieceType.PAWN;
        }

        public List<IMoveSet> Tiers()
        {
            return tiers;
        }
    }
}
