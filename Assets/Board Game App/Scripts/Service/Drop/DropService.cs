using Data.Enums.Piece.Side;
using Data.Enums.Player;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Piece.Find;
using Service.Piece.Set;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Service.Drop
{
    public class DropService
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();

        public void DropPiece(
            ref PieceEV pieceToDrop,
            ref TileEV destinationTile,
            PieceSide side,
            PlayerColor playerOwner,
            IEntitiesDB entitiesDB)
        {
            Vector2 location = destinationTile.Location.Location;

            List<PieceEV> piecesAtLocation = pieceFindService.FindPiecesByLocation(location, entitiesDB);

            if (piecesAtLocation.Count > 0)
            {
                pieceSetService.SetTopOfTowerToFalse(piecesAtLocation[piecesAtLocation.Count - 1], entitiesDB);
            }

            pieceSetService.SetPiecePlayerOwner(pieceToDrop, playerOwner, entitiesDB);
            pieceSetService.SetPieceLocationAndTier(pieceToDrop, location, piecesAtLocation.Count + 1, entitiesDB);
            pieceSetService.SetPieceSide(pieceToDrop, side, entitiesDB);
            pieceToDrop.MovePiece.NewLocation = location;
            pieceToDrop.Visibility.IsVisible.value = true;
            pieceToDrop.ChangeColorTrigger.PlayChangeColor = true;
        }
    }
}
