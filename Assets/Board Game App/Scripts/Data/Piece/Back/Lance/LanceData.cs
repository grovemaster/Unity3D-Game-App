using Data.Enum.Piece;
using System.Collections.Generic;

namespace Data.Piece.Back.Lance
{
    public class LanceData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static LanceData()
        {
            abilities = new LanceAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new LanceMoveSetTier1st(), new LanceMoveSetTier2nd(), new LanceMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.LANCE;
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
