using Data.Enum.Piece.Drop;

namespace Data.Piece
{
    public interface IAbilities
    {
        DropAbility? Drop(); // Piece has maximum of one drop ability
    }
}
