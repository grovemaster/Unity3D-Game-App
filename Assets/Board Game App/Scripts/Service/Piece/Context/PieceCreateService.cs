using Data.Constants.Board;
using Data.Enums.Piece;
using Data.Enums.Piece.Side;
using Data.Enums.Player;
using ECS.EntityDescriptor.Piece;
using ECS.Implementor;
using ECS.Implementor.Hand;
using ECS.Implementor.Piece;
using PrefabUtil;
using Service.Common;
using Service.Directions;
using Svelto.ECS;
using UnityEngine;

namespace Service.Piece.Context
{
    class PieceCreateService
    {
        private IEntityFactory entityFactory;
        private PrefabsDictionary prefabsDictionary;

        public PieceCreateService(IEntityFactory entityFactory)
        {
            this.entityFactory = entityFactory;
            prefabsDictionary = new PrefabsDictionary();
        }

        public void CreatePiece(
            PlayerColor playerOwner,
            PieceType front,
            PieceType back,
            PieceSide side,
            int fileNum,
            int rankNum)
        {
            var piece = prefabsDictionary.Instantiate("Piece");
            var pieceImpl = piece.GetComponent<PieceImpl>();
            var pieceLocationMoveImpl = piece.GetComponent<PieceLocationMoveImpl>();
            var pieceHighlightOwnerImpl = piece.GetComponent<PieceHighlightOwnerImpl>();
            entityFactory.BuildEntity<PieceED>(piece.GetInstanceID(), piece.GetComponents<IImplementor>());

            piece.name = "Piece " + front.ToString() + " " + back.ToString();
            piece.tag = "Piece";

            pieceImpl.Front = front;
            pieceImpl.Back = back;
            pieceImpl.PieceType = side == PieceSide.FRONT ? front : back;
            pieceImpl.Direction = DirectionService.CalcDirection(playerOwner);
            pieceHighlightOwnerImpl.PlayerColor = playerOwner;

            Vector2 location = CommonService.CalcTransformPosition(fileNum, rankNum);
            piece.transform.position = new Vector3(location.x, location.y, 1);
            pieceLocationMoveImpl.Location = new Vector2(fileNum, rankNum);
            pieceHighlightOwnerImpl.PlayChangeColor = true;
        }

        public void MovePieceToHand(GameObject piece, GameObject handPiece)
        {
            var pieceImpl = piece.GetComponent<PieceImpl>();
            var pieceLocationMoveImpl = piece.GetComponent<PieceLocationMoveImpl>();

            piece.transform.position = new Vector3(-500, -500, 1);
            pieceLocationMoveImpl.Location = BoardConst.HAND_LOCATION;

            var handPieceImpl = handPiece.GetComponent<HandPieceImpl>();
            handPieceImpl.NumPieces.value++;
        }
    }
}
