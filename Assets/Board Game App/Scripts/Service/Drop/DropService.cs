using Data.Enum.Piece.Side;
using Data.Enum.Player;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Piece.Set;
using Svelto.ECS;
using UnityEngine;

namespace Service.Drop
{
    public class DropService
    {
        private PieceSetService pieceSetService = new PieceSetService();

        public void DropPiece(
            ref PieceEV pieceToDrop,
            ref TileEV destinationTile,
            PieceSide side,
            PlayerColor playerOwner,
            IEntitiesDB entitiesDB)
        {
            Vector2 location = destinationTile.Location.Location;

            pieceSetService.SetPieceLocationAndTier(pieceToDrop, location, 1, entitiesDB);
            pieceSetService.SetPiecePlayerOwner(pieceToDrop, playerOwner, entitiesDB);
            pieceSetService.SetPieceSide(pieceToDrop, side, entitiesDB);
            pieceToDrop.MovePiece.NewLocation = location;
            pieceToDrop.Visibility.IsVisible.value = true;
            pieceToDrop.ChangeColorTrigger.PlayChangeColor = true;
        }
    }
}
