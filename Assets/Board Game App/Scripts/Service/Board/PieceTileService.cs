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
            ref BoardPressState token,
            out PieceEV? pieceEV,
            out TileEV? tileEV)
        {
            pieceEV = PieceService.FindPieceEVById(token.pieceEntityId, entitiesDB);
            tileEV = TileService.FindTileEVById(token.tileEntityId, entitiesDB);

            if (pieceEV == null
                && tileEV != null
                && ((TileEV)tileEV).tile.PieceRefEntityId != null
                && ((TileEV)tileEV).tile.PieceRefEntityId != 0) // Find by tile information
            {
                pieceEV = PieceService.FindPieceEVById((int)((TileEV)tileEV).tile.PieceRefEntityId, entitiesDB);
            }

            if (tileEV == null) // Find by piece information
            {
                var location = ((PieceEV)pieceEV).location;
                tileEV = TileService.FindTileEV(
                    new Vector3(location.Location.x, location.Location.y, 0),
                    entitiesDB);
            }

            if (tileEV != null && pieceEV == null)
            {
                pieceEV = PieceService.FindPieceByLocation(((TileEV)tileEV).location.Location, entitiesDB);
            }
        }
    }
}
