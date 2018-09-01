using Data.Enums.Piece;
using System.Collections.Generic;

namespace Data.Piece.Back.DragonKing
{
    public class DragonKingData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static DragonKingData()
        {
            abilities = new DragonKingAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new DragonKingMoveSetTier1st(), new DragonKingMoveSetTier2nd(), new DragonKingMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.DRAGON_KING;
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
