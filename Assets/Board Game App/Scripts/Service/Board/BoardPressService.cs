using Data.Enum;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Scripts.Data.Board;

namespace Service.Board
{
    public static class BoardPressService
    {
        public static BoardPress DecideAction(BoardPressStateInfo stateInfo, TurnEV currentTurn)
        {
            BoardPress returnValue = BoardPress.NOTHING;
            TileEV tileEV = stateInfo.tile.Value;
            int tilePieceId = tileEV.tile.PieceRefEntityId.GetValueOrDefault();
            // TODO Scenario: Clicked highlighted tile containing opponent piece to initiate mobile capture

            // If hand piece highlighted and user clicks on empty tile
            // Note: (Future) validation of that tile is done in later engine
            if (stateInfo.handPiece.HasValue
                && !stateInfo.pieceAtDestination.HasValue)
            {
                returnValue = BoardPress.DROP;
            }
            // Tile is clicked, tile highlighted, piece reference exists
            else if (tileEV.highlight.IsHighlighted
                && stateInfo.piece.HasValue
                && tilePieceId != 0
                && stateInfo.piece.Value.playerOwner.PlayerColor == currentTurn.TurnPlayer.PlayerColor)
            {
                returnValue = BoardPress.MOVE_PIECE;

                // If opponent piece in destination tile
                // TODO Later on, differentiate by pieceToMove and destinationTileLocation to determine mobile vs immobile capture
                if (stateInfo.pieceAtDestination.HasValue
                    && stateInfo.pieceAtDestination.Value.playerOwner.PlayerColor != currentTurn.TurnPlayer.PlayerColor)
                {
                    returnValue = BoardPress.MOBILE_CAPTURE;
                }
            }
            // NOT move scenario, Piece/Tile clicked, both piece and tile exist, piece reference does not exist
            else if (stateInfo.piece.HasValue && tilePieceId == 0)
            {
                returnValue = BoardPress.CLICK_HIGHLIGHT;
            }

            return returnValue;
        }
    }
}
