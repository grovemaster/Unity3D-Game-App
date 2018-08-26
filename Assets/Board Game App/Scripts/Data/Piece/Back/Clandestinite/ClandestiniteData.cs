using Data.Enum.Piece;
using System.Collections.Generic;

namespace Data.Piece.Back.Clandestinite
{
    public class ClandestiniteData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static ClandestiniteData()
        {
            abilities = new ClandestiniteAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new ClandestiniteMoveSetTier1st(), new ClandestiniteMoveSetTier2nd(), new ClandestiniteMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.CLANDESTINITE;
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
