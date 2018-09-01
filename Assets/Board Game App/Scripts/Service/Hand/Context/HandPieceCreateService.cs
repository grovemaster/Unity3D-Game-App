using Data.Constants.Board;
using Data.Enums.Piece;
using Data.Enums.Player;
using ECS.EntityDescriptor.Hand;
using ECS.Implementor;
using ECS.Implementor.Hand;
using PrefabUtil;
using Svelto.ECS;
using UnityEngine;

namespace Service.Hand.Context
{
    class HandPieceCreateService
    {
        private IEntityFactory entityFactory;
        private PrefabsDictionary prefabsDictionary;

        public HandPieceCreateService(IEntityFactory entityFactory)
        {
            this.entityFactory = entityFactory;
            prefabsDictionary = new PrefabsDictionary();
        }

        public void CreateHandPiece(PlayerColor playerOwner, PieceType front, PieceType back, int num)
        {
            var handPiece = prefabsDictionary.Instantiate("Hand Piece");
            var handPieceImpl = handPiece.GetComponent<HandPieceImpl>();
            entityFactory.BuildEntity<HandPieceED>(handPiece.GetInstanceID(), handPiece.GetComponents<IImplementor>());

            handPieceImpl.PieceType = front;
            handPieceImpl.Back = back;
            handPieceImpl.PlayerColor = playerOwner;

            // TODO Abstract out offset position numbers into BoardConst later -- once I have those numbers.
            // TODO Adjust transform.position based on, well, position of other hand pieces of that player
            Vector3 handPieceLocation;

            if (playerOwner == PlayerColor.BLACK)
            {
                handPieceLocation = BoardConst.HAND_PIECE_BLACK_OFFSET;
                handPieceLocation = new Vector3(
                    handPieceLocation.x + BoardConst.HAND_PIECE_X_SPACE * (num % 7),
                    handPieceLocation.y - BoardConst.HAND_PIECE_Y_SPACE * (num / 7),
                    handPieceLocation.z
                    );
            }
            else
            {
                handPieceLocation = BoardConst.HAND_PIECE_WHITE_OFFSET;
                handPieceLocation = new Vector3(
                    handPieceLocation.x + BoardConst.HAND_PIECE_X_SPACE * (num % 7),
                    handPieceLocation.y + BoardConst.HAND_PIECE_Y_SPACE * (num / 7),
                    handPieceLocation.z
                    );
            }

            handPiece.transform.position = handPieceLocation;
        }
    }
}
