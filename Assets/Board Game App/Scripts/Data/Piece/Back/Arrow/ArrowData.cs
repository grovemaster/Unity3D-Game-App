using Data.Enum.Piece;
using Data.Piece.Default;
using System.Collections.Generic;

namespace Data.Piece.Back.Arrow
{
    public class ArrowData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static ArrowData()
        {
            abilities = new NoAbility();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new ArrowMoveSetTier1st(), new ArrowMoveSetTier2nd(), new ArrowMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.ARROW;
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
