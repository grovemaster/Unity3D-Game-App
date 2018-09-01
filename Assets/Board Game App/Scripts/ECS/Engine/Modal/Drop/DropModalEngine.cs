using Data.Enums.Piece;
using Data.Enums.Modal;
using Data.Step.Drop;
using ECS.EntityView.Modal;
using Service.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal.Drop
{
    class DropModalEngine : IStep<DropPrepStepState>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref DropPrepStepState token, int condition)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);

            SetModalOptions(modal, ref token);
            modalService.DisplayModal(modal);
        }

        private void SetModalOptions(ModalEV modal, ref DropPrepStepState token)
        {
            int tileReferenceId = token.DestinationTile.ID.entityID;
            int handPieceReferenceId = token.HandPiece.ID.entityID;
            PieceType front = token.HandPiece.HandPiece.PieceType;
            PieceType back = token.HandPiece.HandPiece.Back;

            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modal.Type.Type = ModalType.FRONT_BACK;
                    modal.DropFrontBackModal.TileReferenceId = tileReferenceId;
                    modal.DropFrontBackModal.HandPieceReferenceId = handPieceReferenceId;
                    modal.DropFrontBackModal.Front = front;
                    modal.DropFrontBackModal.Back = back;
                });
        }
    }
}
