using Data.Enum.Piece;
using System.Collections.Generic;

namespace Data.Piece.Front.Spy
{
    public class SpyData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static SpyData()
        {
            abilities = new SpyAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new SpyMoveSetTier1st(), new SpyMoveSetTier2nd(), new SpyMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.SPY;
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
