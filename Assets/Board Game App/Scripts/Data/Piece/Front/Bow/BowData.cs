using Data.Enums.Piece;
using Data.Piece.Default;
using System.Collections.Generic;

namespace Data.Piece.Front.Bow
{
    public class BowData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static BowData()
        {
            abilities = new NoAbility();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new BowMoveSetTier1st(), new BowMoveSetTier2nd(), new BowMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.BOW;
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
