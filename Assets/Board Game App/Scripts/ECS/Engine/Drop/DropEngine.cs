using Data.Constants.Board;
using Data.Enum.Player;
using Data.Step.Drop;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Engine.Drop
{
    class DropEngine : IStep<DropStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref DropStepState token, int condition)
        {
            PieceEV pieceToDrop = PieceService.FindFirstPieceByLocationAndType(
                BoardConst.HAND_LOCATION, token.handPiece.HandPiece.PieceType, entitiesDB);

            DropPiece(ref pieceToDrop, ref token.destinationTile, token.handPiece.PlayerOwner.PlayerColor);
            UpdateHandPiece(ref token.handPiece);
        }

        private void DropPiece(ref PieceEV pieceToDrop, ref TileEV destinationTile, PlayerColor playerOwner)
        {
            Vector2 location = destinationTile.Location.Location;

            PieceService.SetPieceLocationAndTier(pieceToDrop, location, 1, entitiesDB);
            PieceService.SetPiecePlayerOwner(pieceToDrop, playerOwner, entitiesDB);
            pieceToDrop.MovePiece.NewLocation = location;
            pieceToDrop.Visibility.IsVisible.value = true;
        }

        private void UpdateHandPiece(ref HandPieceEV handPiece)
        {
            handPiece.HandPiece.NumPieces.value--;
        }
    }
}
