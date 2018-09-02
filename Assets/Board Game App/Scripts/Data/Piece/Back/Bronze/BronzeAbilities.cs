using Data.Enums.Piece.Drop;
using Data.Enums.Piece.OtherMove;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using System.Collections.Generic;

namespace Data.Piece.Back.Bronze
{
    class BronzeAbilities : IAbilities
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
                return new List<PreMoveAbility>(new PreMoveAbility[] { PreMoveAbility.TWO_FILE_MOVE });
            }
        }

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
                return new List<PostMoveAbility>(new PostMoveAbility[] { PostMoveAbility.BETRAYAL });
            }
        }
    }
}
