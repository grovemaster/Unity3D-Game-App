using System.Collections.Generic;
using Data.Enums.Piece;

namespace Data.Piece.Front.Samurai
{
    public class SamuraiData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static SamuraiData()
        {
            abilities = new SamuraiAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new SamuraiMoveSetTier1st(), new SamuraiMoveSetTier2nd(), new SamuraiMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.SAMURAI;
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
