using Data.Enums.Piece;
using System.Collections.Generic;

namespace Data.Piece.Front.Prodigy
{
    public class ProdigyData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static ProdigyData()
        {
            abilities = new ProdigyAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new ProdigyMoveSetTier1st(), new ProdigyMoveSetTier2nd(), new ProdigyMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.PRODIGY;
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
