using Data.Enums;
using Data.Enums.Player;

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
