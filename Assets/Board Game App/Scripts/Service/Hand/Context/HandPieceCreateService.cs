using Data.Constants.Board;
using Data.Enum;
using Data.Enum.Player;
using ECS.EntityDescriptor.Hand;
using ECS.Implementor;
using ECS.Implementor.Hand;
using PrefabUtil;
using Svelto.ECS;

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

        public void CreateHandPiece(PlayerColor playerOwner, PieceType pieceType)
        {
            var handPiece = prefabsDictionary.Instantiate("Hand Piece");
            var handPieceImpl = handPiece.GetComponent<HandPieceImpl>();
            entityFactory.BuildEntity<HandPieceED>(handPiece.GetInstanceID(), handPiece.GetComponents<IImplementor>());

            handPieceImpl.PieceType = pieceType;
            handPieceImpl.PlayerColor = playerOwner;

            // TODO Abstract out offset position numbers into BoardConst later -- once I have those numbers.
            if (playerOwner == PlayerColor.BLACK)
            {
                handPiece.transform.position = BoardConst.HAND_PIECE_BLACK_OFFSET;
            }
            else
            {
                handPiece.transform.position = BoardConst.HAND_PIECE_WHITE_OFFSET;
            }
        }
    }
}
