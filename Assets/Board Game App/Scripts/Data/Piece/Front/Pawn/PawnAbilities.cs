using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;

namespace Data.Piece.Front.Pawn
{
    public class PawnAbilities : IAbilities
    {
        public DropAbility? Drop
        {
            get
            {
                return DropAbility.DOUBLE_PAWN_DROP;
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
