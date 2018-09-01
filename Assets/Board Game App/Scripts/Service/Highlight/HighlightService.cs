using Data.Enums;
using Data.Enums.Player;

namespace Service.Highlight
{
    public static class HighlightService
    {
        public static HighlightState CalcClickHighlightState(PlayerColor playerColor)
        {
            return playerColor == PlayerColor.BLACK
                ? HighlightState.PLAYER_BLACK_CLICK_HIGHLIGHT : HighlightState.PLAYER_WHITE_CLICK_HIGHLIGHT;
        }

        public static HighlightState CalcRangeHighlightState(PlayerColor playerColor)
        {
            return playerColor == PlayerColor.BLACK
                ? HighlightState.PLAYER_BLACK_RANGE_HIGHLIGHT : HighlightState.PLAYER_WHITE_RANGE_HIGHLIGHT;
        }
    }
}
