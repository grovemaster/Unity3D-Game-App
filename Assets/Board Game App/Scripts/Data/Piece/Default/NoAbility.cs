using Data.Enum.Piece.Drop;

namespace Data.Piece.Default
{
    public class NoAbility : IAbilities
    {
        public DropAbility? Drop()
        {
            return null;
        }
    }
}
