using Data.Enum;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;

namespace Service.Board
{
    public static class BoardPressService
    {
        public static BoardPress DecideAction(
            PieceEV? pieceEV, TileEV? tileEVParam, PieceEV? pieceAtDestination, TurnEV currentTurn)
        {
            BoardPress returnValue = BoardPress.NOTHING;
            TileEV tileEV = tileEVParam.Value;
            int tilePieceId = tileEV.tile.PieceRefEntityId.GetValueOrDefault();
            // TODO Scenario: Clicked highlighted tile containing opponent piece to initiate mobile capture

            // Tile is clicked, tile highlighted, piece reference exists
            if (tileEV.highlight.IsHighlighted
                && pieceEV.HasValue
                && tilePieceId != 0
                && pieceEV.Value.playerOwner.PlayerColor == currentTurn.TurnPlayer.PlayerColor)
            {
                returnValue = BoardPress.MOVE_PIECE;

                // If opponent piece in destination tile
                // TODO Later on, differentiate by pieceToMove and destinationTileLocation to determine mobile vs immobile capture
                if (pieceAtDestination.HasValue
                    && pieceAtDestination.Value.playerOwner.PlayerColor != currentTurn.TurnPlayer.PlayerColor)
                {
                    returnValue = BoardPress.MOBILE_CAPTURE;
                }
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
