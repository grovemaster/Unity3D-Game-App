using Data.Enums.Piece.Drop;
using Data.Enums.Piece.PostMove;
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

        public List<PostMoveAbility> PostMove
        {
            get
            {
                return new List<PostMoveAbility>();
            }
        }
    }
}
