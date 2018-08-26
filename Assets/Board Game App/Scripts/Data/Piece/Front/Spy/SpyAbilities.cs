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
                return null;
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
