using Data.Enum.Modal;
using Data.Step.Piece.Capture;
using ECS.EntityView.Modal;
using Service.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal.CaptureStack
{
    class CaptureStackModalEngine : IStep<CapturePieceStepState>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref CapturePieceStepState token, int condition)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            SetModalOptions(modal, ref token);
            modalService.DisplayModal(modal);
        }

        private void SetModalOptions(ModalEV modal, ref CapturePieceStepState token)
        {
            int tileReferenceId = token.DestinationTile.ID.entityID;

            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modal.Type.Type = ModalType.CAPTURE_STACK;
                    modal.CaptureOrStack.TileReferenceId = tileReferenceId;
                });
        }
    }
}
