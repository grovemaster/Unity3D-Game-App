using Data.Enum;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;

namespace Service.Board
{
    public static class BoardPressService
    {
        public static BoardPress DecideAction(PieceEV? pieceEV, TileEV? tileEVParam)
        {
            BoardPress returnValue = BoardPress.NOTHING;
            TileEV tileEV = (TileEV)tileEVParam;
            int tilePieceId = tileEV.tile.PieceRefEntityId ?? 0;

            // Tile is clicked, tile highlighted, piece reference exists
            if (tileEV.highlight.IsHighlighted && pieceEV != null && tilePieceId != 0)
            {
                returnValue = BoardPress.MOVE_PIECE;
            }
            // NOT move scenario, Piece/Tile clicked, both piece and tile exist, piece reference does not exist
            else if (pieceEV != null && tilePieceId == 0)
            {
                returnValue = BoardPress.CLICK_HIGHLIGHT;
            }

            return returnValue;
        }
    }
}
