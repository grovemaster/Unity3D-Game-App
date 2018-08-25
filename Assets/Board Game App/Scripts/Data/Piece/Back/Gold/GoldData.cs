using Data.Enum.Piece;
using Data.Piece.Default;
using System.Collections.Generic;

namespace Data.Piece.Back.Gold
{
    class GoldData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static GoldData()
        {
            abilities = new NoAbility();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new GoldMoveSetTier1st(), new GoldMoveSetTier2nd(), new GoldMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.GOLD;
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
