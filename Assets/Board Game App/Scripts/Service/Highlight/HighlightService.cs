using Data.Enum;
using Data.Enum.Player;

namespace Service.Highlight
{
    public class HighlightService
    {
        public static HighlightState CalcHighlightState(PlayerColor playerColor)
        {
            return playerColor == PlayerColor.BLACK
                ? HighlightState.PLAYER_BLACK_CLICK_HIGHLIGHT : HighlightState.PLAYER_WHITE_CLICK_HIGHLIGHT;
        }
    }
}
