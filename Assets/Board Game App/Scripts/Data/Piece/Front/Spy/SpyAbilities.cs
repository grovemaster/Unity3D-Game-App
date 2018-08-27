using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;
using System.Collections.Generic;

namespace Data.Piece.Front.Spy
{
    class SpyAbilities : IAbilities
    {
        public List<DropAbility> Drop
        {
            get
            {
                return new List<DropAbility>(new DropAbility[] { DropAbility.EARTH_LINK_BACK });
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
