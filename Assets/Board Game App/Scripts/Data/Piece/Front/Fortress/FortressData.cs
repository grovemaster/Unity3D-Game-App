using Data.Enums.Piece;
using System.Collections.Generic;

namespace Data.Piece.Front.Fortress
{
    class FortressData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static FortressData()
        {
            abilities = new FortressAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new FortressMoveSetTier1st(), new FortressMoveSetTier2nd(), new FortressMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.FORTRESS;
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
