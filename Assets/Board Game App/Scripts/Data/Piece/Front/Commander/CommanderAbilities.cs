using Data.Enums.Piece.Drop;
using Data.Enums.Piece.OtherMove;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using System.Collections.Generic;

namespace Data.Piece.Front.Commander
{
    class CommanderAbilities : IAbilities
    {
        public OtherMoveAbility? OtherMove
        {
            get
            {
                return OtherMoveAbility.TIER_3_EXCHANGE;
            }
        }

        public List<PreMoveAbility> PreMove
        {
            get
            {
                return new List<PreMoveAbility>(new PreMoveAbility[]
                {
                    PreMoveAbility.CANNOT_BE_STACKED,
                    PreMoveAbility.CANNOT_MOBILE_RANGE_EXPANSION
                });
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
                return new List<PostMoveAbility>();
            }
        }
    }
}
