using Data.Enums.Piece;
using System.Collections.Generic;

namespace Data.Piece.Front.Commander
{
    public class CommanderData : IPieceData
    {
        private static readonly IAbilities abilities;
        private static readonly List<IMoveSet> tiers;

        static CommanderData()
        {
            abilities = new CommanderAbilities();
            tiers = new List<IMoveSet>(new IMoveSet[]
            { new CommanderMoveSetTier1st(), new CommanderMoveSetTier2nd(), new CommanderMoveSetTier3rd() });
        }

        public PieceType TypeOfPiece
        {
            get
            {
                return PieceType.COMMANDER;
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
