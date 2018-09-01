using Data.Enums.Piece.Drop;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using System.Collections.Generic;

namespace Data.Piece.Back.Clandestinite
{
    class ClandestiniteAbilities : IAbilities
    {
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
                return new List<DropAbility>(new DropAbility[] { DropAbility.EARTH_LINK_FRONT });
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
