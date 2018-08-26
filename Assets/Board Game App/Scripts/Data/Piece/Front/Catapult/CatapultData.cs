using Data.Enum.Piece;
using System.Collections.Generic;

namespace Data.Piece.Front.Catapult
{
    public class CatapultData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static CatapultData()
        {
            abilities = new CatapultAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new CatapultMoveSetTier1st(), new CatapultMoveSetTier2nd(), new CatapultMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.CATAPULT;
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
