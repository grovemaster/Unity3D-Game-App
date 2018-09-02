using Data.Enums.Piece;
using Data.Piece.Default;
using System.Collections.Generic;

namespace Data.Piece.Back.Pistol
{
    public class PistolData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static PistolData()
        {
            abilities = new NoAbility();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new PistolMoveSetTier1st(), new PistolMoveSetTier2nd(), new PistolMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.PISTOL;
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
