using Data.Step.Board;
using ECS.EntityView.Turn;
using Scripts.Data.Board;
using Service.Board.Tile;
using Service.Hand;
using Service.Piece;
using Svelto.ECS;
using UnityEngine;

namespace Service.Board
{
    public class PieceTileService
    {
        private HandService handService = new HandService();

        public BoardPressStateInfo FindBoardPressStateInfo(IEntitiesDB entitiesDB, ref BoardPressStepState token)
        {
            BoardPressStateInfo returnValue = new BoardPressStateInfo
            {
                piece = PieceService.FindPieceEVById(token.pieceEntityId, entitiesDB),
                tile = TileService.FindTileEVById(token.tileEntityId, entitiesDB),
                pieceAtDestination = null, // If movement-related information is later required
                handPiece = handService.FindHighlightedHandPiece(entitiesDB)
            };

            if (!returnValue.piece.HasValue
                && returnValue.tile.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId != 0) // Find by tile information
            {
                returnValue.piece = PieceService.FindPieceEVById(
                    returnValue.tile.Value.Tile.PieceRefEntityId.Value, entitiesDB);
            }

            if (!returnValue.tile.HasValue) // Find by piece information
            {
                returnValue.tile = TileService.FindTileEV(
                    returnValue.piece.Value.Location.Location, entitiesDB);
            }

            if (returnValue.tile.HasValue && !returnValue.piece.HasValue)
            {
                returnValue.piece = PieceService.FindTopPieceByLocation(
                    returnValue.tile.Value.Location.Location, entitiesDB);
            }

            if (returnValue.tile.HasValue)
            {
                returnValue.pieceAtDestination = PieceService.FindTopPieceByLocation(
                    returnValue.tile.Value.Location.Location, entitiesDB);
            }

            // Movement: Piece is clicked with intention to move a DIFFERENT piece to that tile location
            if (returnValue.tile.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId.Value != 0
                && returnValue.piece.HasValue
                && returnValue.piece.Value.Location.Location == returnValue.tile.Value.Location.Location)
            {
                returnValue.piece = PieceService.FindPieceEVById(
                    returnValue.tile.Value.Tile.PieceRefEntityId.Value, entitiesDB);
            }

            return returnValue;
        }
    }
}
