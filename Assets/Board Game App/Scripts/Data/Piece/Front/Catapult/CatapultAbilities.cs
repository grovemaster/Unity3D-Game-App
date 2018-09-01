using Data.Enums.Piece.Drop;
using Data.Enums.Piece.PostMove;
using System.Collections.Generic;

namespace Data.Piece.Front.Catapult
{
    class CatapultAbilities : IAbilities
    {
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
