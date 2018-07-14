using Data.Enum;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;

namespace Service.Board
{
    public static class BoardPressService
    {
        public static BoardPress DecideAction(PieceEV? pieceEV, TileEV? tileEVParam, TurnEV currentTurn)
        {
            BoardPress returnValue = BoardPress.NOTHING;
            TileEV tileEV = tileEVParam.Value;
            int tilePieceId = tileEV.tile.PieceRefEntityId ?? 0;

            // Tile is clicked, tile highlighted, piece reference exists
            if (tileEV.highlight.IsHighlighted
                && pieceEV.HasValue
                && tilePieceId != 0
                && pieceEV.Value.playerOwner.PlayerColor.Equals(currentTurn.TurnPlayer.PlayerColor))
            {
                returnValue = BoardPress.MOVE_PIECE;
            }
            // NOT move scenario, Piece/Tile clicked, both piece and tile exist, piece reference does not exist
            else if (pieceEV.HasValue && tilePieceId == 0)
            {
                returnValue = BoardPress.CLICK_HIGHLIGHT;
            }

            return returnValue;
        }
    }
}
