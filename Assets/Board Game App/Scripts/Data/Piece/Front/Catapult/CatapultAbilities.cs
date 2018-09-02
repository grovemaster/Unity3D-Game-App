using Data.Enums.Piece.Drop;
using Data.Enums.Piece.OtherMove;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using System.Collections.Generic;

namespace Data.Piece.Front.Catapult
{
    class CatapultAbilities : IAbilities
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
                return new List<PreMoveAbility>(new PreMoveAbility[] { PreMoveAbility.MOBILE_RANGE_EXPANSION_RADIAL });
            }
        }

        public List<DropAbility> Drop
        {
            get
            {
                return new List<DropAbility>(new DropAbility[] { DropAbility.EARTH_LINK, DropAbility.TERRITORY_DROP });
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
