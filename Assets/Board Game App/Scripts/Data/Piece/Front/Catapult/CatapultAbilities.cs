using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;

namespace Data.Piece.Front.Catapult
{
    class CatapultAbilities : IAbilities
    {
        public DropAbility? Drop
        {
            get
            {
                return DropAbility.TERRITORY_DROP;
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
