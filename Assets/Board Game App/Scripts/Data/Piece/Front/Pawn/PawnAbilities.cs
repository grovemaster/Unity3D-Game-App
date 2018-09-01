using Data.Enums.Piece.Drop;
using Data.Enums.Piece.PostMove;
using System.Collections.Generic;

namespace Data.Piece.Front.Pawn
{
    class PawnAbilities : IAbilities
    {
        public List<DropAbility> Drop
        {
            get
            {
                return new List<DropAbility>(new DropAbility[] { DropAbility.DOUBLE_FILE_DROP });
            }
        }

        public List<PostMoveAbility> PostMove
        {
            get
            {
                return new List<PostMoveAbility>(new PostMoveAbility[] { PostMoveAbility.FORCED_RECOVERY });
            }
        }
    }
}
