using Data.Enum.Piece;
using Data.Piece.Default;
using System.Collections.Generic;

namespace Data.Piece.Back.Bronze
{
    class BronzeData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static BronzeData()
        {
            abilities = new NoAbility();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new BronzeMoveSetTier1st(), new BronzeMoveSetTier2nd(), new BronzeMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.BRONZE;
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
