using Data.Enum;
using Data.Enum.Player;

namespace Service.Directions
{
    public static class DirectionService
    {
        public static Direction CalcDirection(PlayerColor playerOwner)
        {
            return playerOwner == PlayerColor.BLACK ? Direction.UP : Direction.DOWN;
        }
    }
}
