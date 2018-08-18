using System.Collections.Generic;
using Data.Enum;

namespace Data.Piece.Front.Pawn
{
    class PawnData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static PawnData()
        {
            abilities = new PawnAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new PawnMoveSetTier1st(), new PawnMoveSetTier2nd(), new PawnMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.PAWN;
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
