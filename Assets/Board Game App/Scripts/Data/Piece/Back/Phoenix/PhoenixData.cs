using Data.Enums.Piece;
using System.Collections.Generic;

namespace Data.Piece.Back.Phoenix
{
    public class PhoenixData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static PhoenixData()
        {
            abilities = new PhoenixAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new PhoenixMoveSetTier1st(), new PhoenixMoveSetTier2nd(), new PhoenixMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.PHOENIX;
            }
        }

        public IAbilities Abilities
        {
            get
            {
                return abilities;
            }
        }

        public List<IMoveSet> Tiers
        {
            get
            {
                return tiers;
            }
        }
    }
}
