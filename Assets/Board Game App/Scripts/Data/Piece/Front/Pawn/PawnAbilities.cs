using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;

namespace Data.Piece.Front.Pawn
{
    class PawnAbilities : IAbilities
    {
        public DropAbility? Drop
        {
            get
            {
                return DropAbility.DOUBLE_FILE_DROP;
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
