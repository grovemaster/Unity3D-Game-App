using Data.Enums.Piece;
using Data.Piece.Default;
using System.Collections.Generic;

namespace Data.Piece.Back.Silver
{
    public class SilverData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static SilverData()
        {
            abilities = new NoAbility();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new SilverMoveSetTier1st(), new SilverMoveSetTier2nd(), new SilverMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.SILVER;
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
