using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;
using System.Collections.Generic;

namespace Data.Piece.Front.Commander
{
    class CommanderAbilities : IAbilities
    {
        public List<DropAbility> Drop
        {
            get
            {
                return new List<DropAbility>();
            }
        }

        public PostMoveAbility? PostMove
        {
            get
            {
                return null;
            }
        }
    }
}
