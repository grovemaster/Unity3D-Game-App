using Data.Enum.Piece.Drop;

namespace Data.Piece.Front.Pawn
{
    public class PawnAbilities : IAbilities
    {
        public DropAbility? Drop()
        {
            return DropAbility.DOUBLE_PAWN_DROP;
        }
    }
}
