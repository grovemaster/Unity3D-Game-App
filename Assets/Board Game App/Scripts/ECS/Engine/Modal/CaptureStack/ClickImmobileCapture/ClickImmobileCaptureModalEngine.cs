using Data.Enums.Modal;
using Data.Step.Modal;
using ECS.EntityView.Modal;
using Service.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal.CaptureStack.ClickImmobileCapture
{
    class ClickImmobileCaptureModalEngine : IStep<ClickImmobileCaptureStepState>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref ClickImmobileCaptureStepState token, int condition)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            SetModalOptions(modal, ref token);
            modalService.DisplayModal(modal);
        }

        private void SetModalOptions(ModalEV modal, ref ClickImmobileCaptureStepState token)
        {
            int tileReferenceId = token.TileReferenceEV.ID.entityID;

            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modal.Type.Type = ModalType.CLICK_IMMOBILE_CAPTURE;
                    modal.CaptureOrStack.TileReferenceId = tileReferenceId;
                });
        }
    }
}
