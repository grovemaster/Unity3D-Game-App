using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;

namespace Data.Piece.Front.Spy
{
    class SpyAbilities : IAbilities
    {
        public DropAbility? Drop
        {
            get
            {
                return DropAbility.EARTH_LINK_BACK;
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
