using Data.Step.Board;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Board.Tile;
using Service.Piece;
using Svelto.ECS;
using UnityEngine;

namespace Service.Board
{
    public static class PieceTileService
    {
        public static void FindPieceTileEV(
            IEntitiesDB entitiesDB,
            ref BoardPressStepState token,
            out PieceEV? pieceEV,
            out TileEV? tileEV,
            out PieceEV? pieceAtDestination)
        {
            pieceEV = PieceService.FindPieceEVById(token.pieceEntityId, entitiesDB);
            tileEV = TileService.FindTileEVById(token.tileEntityId, entitiesDB);
            pieceAtDestination = null; // If movement-related information is later required

            if (!pieceEV.HasValue
                && tileEV.HasValue
                && tileEV.Value.tile.PieceRefEntityId.HasValue
                && tileEV.Value.tile.PieceRefEntityId != 0) // Find by tile information
            {
                pieceEV = PieceService.FindPieceEVById(tileEV.Value.tile.PieceRefEntityId.Value, entitiesDB);
            }

            if (!tileEV.HasValue) // Find by piece information
            {
                var location = pieceEV.Value.location;
                tileEV = TileService.FindTileEV(
                    new Vector3(location.Location.x, location.Location.y, 0),
                    entitiesDB);
            }

            if (tileEV.HasValue && !pieceEV.HasValue)
            {
                pieceEV = PieceService.FindPieceByLocation(tileEV.Value.location.Location, entitiesDB);
            }

            if (tileEV.HasValue)
            {
                pieceAtDestination = PieceService.FindPieceByLocation(tileEV.Value.location.Location, entitiesDB);
            }

            // Movement: Piece is clicked with intention to move a DIFFERENT piece to that tile location
            if (tileEV.HasValue
                && tileEV.Value.tile.PieceRefEntityId.HasValue
                && tileEV.Value.tile.PieceRefEntityId.Value != 0
                && pieceEV.HasValue
                && pieceEV.Value.location.Location.x == tileEV.Value.location.Location.x
                && pieceEV.Value.location.Location.y == tileEV.Value.location.Location.y)
            {
                pieceEV = PieceService.FindPieceEVById(tileEV.Value.tile.PieceRefEntityId.Value, entitiesDB);
            }
        }
    }
}
