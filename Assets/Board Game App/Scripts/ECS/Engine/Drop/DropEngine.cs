using Data.Constants.Board;
using Data.Enum.Player;
using Data.Step.Drop;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Piece;
using Service.Turn;
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
                BoardConst.HAND_LOCATION, token.handPiece.handPiece.PieceType, entitiesDB);

            DropPiece(ref pieceToDrop, ref token.destinationTile, token.handPiece.playerOwner.PlayerColor);
            UpdateHandPiece(ref token.handPiece);
        }

        private void DropPiece(ref PieceEV pieceToDrop, ref TileEV destinationTile, PlayerColor playerOwner)
        {
            Vector3 location = new Vector3(
                destinationTile.location.Location.x,
                destinationTile.location.Location.y,
                1);

            PieceService.SetPieceLocation(pieceToDrop, location, entitiesDB);
            PieceService.SetPiecePlayerOwner(pieceToDrop, playerOwner, entitiesDB);
            pieceToDrop.movePiece.NewLocation = location;
            pieceToDrop.visibility.IsVisible.value = true;
        }

        private void UpdateHandPiece(ref HandPieceEV handPiece)
        {
            handPiece.handPiece.NumPieces.value--;
        }
    }
}
