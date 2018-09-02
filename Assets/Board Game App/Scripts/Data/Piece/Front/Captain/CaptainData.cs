using System.Collections.Generic;
using Data.Enums.Piece;

namespace Data.Piece.Front.Captain
{
    public class CaptainData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static CaptainData()
        {
            abilities = new CaptainAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new CaptainMoveSetTier1st(), new CaptainMoveSetTier2nd(), new CaptainMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.CAPTAIN;
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
