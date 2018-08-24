using Data.Step.Board;
using Scripts.Data.Board;
using Service.Board.Tile;
using Service.Hand;
using Service.Piece.Find;
using Svelto.ECS;

namespace Service.Board
{
    public class PieceTileService
    {
        private HandService handService = new HandService();
        private PieceFindService pieceFindService = new PieceFindService();
        private TileService tileService = new TileService();

        public BoardPressStateInfo FindBoardPressStateInfo(IEntitiesDB entitiesDB, ref BoardPressStepState token)
        {
            BoardPressStateInfo returnValue = new BoardPressStateInfo
            {
                piece = pieceFindService.FindPieceEVById(token.PieceEntityId, entitiesDB),
                tile = tileService.FindTileEVById(token.TileEntityId, entitiesDB),
                pieceAtDestination = null, // If movement-related information is later required
                handPiece = handService.FindHighlightedHandPiece(entitiesDB)
            };

            if (!returnValue.piece.HasValue
                && returnValue.tile.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId != 0) // Find by tile information
            {
                returnValue.piece = pieceFindService.FindPieceEVById(
                    returnValue.tile.Value.Tile.PieceRefEntityId.Value, entitiesDB);
            }

            if (!returnValue.tile.HasValue) // Find by piece information
            {
                returnValue.tile = tileService.FindTileEV(
                    returnValue.piece.Value.Location.Location, entitiesDB);
            }

            if (returnValue.tile.HasValue && !returnValue.piece.HasValue)
            {
                returnValue.piece = pieceFindService.FindTopPieceByLocation(
                    returnValue.tile.Value.Location.Location, entitiesDB);
            }

            if (returnValue.tile.HasValue)
            {
                returnValue.pieceAtDestination = pieceFindService.FindTopPieceByLocation(
                    returnValue.tile.Value.Location.Location, entitiesDB);
            }

            // Movement: Piece is clicked with intention to move a DIFFERENT piece to that tile location
            if (returnValue.tile.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId.HasValue
                && returnValue.tile.Value.Tile.PieceRefEntityId.Value != 0
                && returnValue.piece.HasValue
                && returnValue.piece.Value.Location.Location == returnValue.tile.Value.Location.Location)
            {
                returnValue.piece = pieceFindService.FindPieceEVById(
                    returnValue.tile.Value.Tile.PieceRefEntityId.Value, entitiesDB);
            }

            return returnValue;
        }
    }
}
