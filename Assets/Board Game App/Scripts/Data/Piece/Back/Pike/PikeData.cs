using Data.Enums.Piece;
using Data.Piece.Default;
using System.Collections.Generic;

namespace Data.Piece.Back.Pike
{
    public class PikeData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static PikeData()
        {
            abilities = new NoAbility();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new PikeMoveSetTier1st(), new PikeMoveSetTier2nd(), new PikeMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.PIKE;
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
