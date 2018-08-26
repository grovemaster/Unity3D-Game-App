using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;

namespace Data.Piece.Back.Clandestinite
{
    class ClandestiniteAbilities : IAbilities
    {
        public DropAbility? Drop
        {
            get
            {
                return DropAbility.EARTH_LINK_FRONT;
            }
        }

        public PostMoveAbility? PostMove
        {
            get
            {
                return PostMoveAbility.FORCED_RECOVERY;
            }
        }
    }
}
