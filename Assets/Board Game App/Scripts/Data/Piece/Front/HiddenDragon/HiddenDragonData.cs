using Data.Enums.Piece;
using System.Collections.Generic;

namespace Data.Piece.Front.HiddenDragon
{
    public class HiddenDragonData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static HiddenDragonData()
        {
            abilities = new HiddenDragonAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new HiddenDragonMoveSetTier1st(), new HiddenDragonMoveSetTier2nd(), new HiddenDragonMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.HIDDEN_DRAGON;
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
