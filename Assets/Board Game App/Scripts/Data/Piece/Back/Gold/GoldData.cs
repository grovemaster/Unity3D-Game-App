using Data.Enum;
using System.Collections.Generic;

namespace Data.Piece.Back.Gold
{
    public class GoldData : IPieceData
    {
        private static readonly List<IMoveSet> tiers;

        static GoldData()
        {
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new GoldMoveSetTier1st(), new GoldMoveSetTier2nd(), new GoldMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece()
        {
            return PieceType.GOLD;
        }

        public List<IMoveSet> Tiers()
        {
            return tiers;
        }
    }
}
