using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;
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

        public PostMoveAbility? PostMove
        {
            get
            {
                return null;
            }
        }
    }
}
