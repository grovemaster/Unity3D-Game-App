using Data.Enums.Piece.Drop;
using Data.Enums.Piece.OtherMove;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using System.Collections.Generic;

namespace Data.Piece.Back.Lance
{
    class LanceAbilities : IAbilities
    {
        public OtherMoveAbility? OtherMove
        {
            get
            {
                return null;
            }
        }

        public List<PreMoveAbility> PreMove
        {
            get
            {
                return new List<PreMoveAbility>();
            }
        }

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
                return new List<PostMoveAbility>(new PostMoveAbility[] { PostMoveAbility.FORCED_RECOVERY, PostMoveAbility.FORCED_REARRANGEMENT });
            }
        }
    }
}
