using Data.Enum.Piece.Drop;
using Data.Enum.Piece.PostMove;

namespace Data.Piece.Default
{
    public class NoAbility : IAbilities
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
                return null;
            }
        }
    }
}
