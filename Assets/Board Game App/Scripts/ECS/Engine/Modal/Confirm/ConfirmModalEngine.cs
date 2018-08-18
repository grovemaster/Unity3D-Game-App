using Data.Enum.Modal;
using Data.Step.Piece.Move;
using ECS.EntityView.Modal;
using Service.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal.Confirm
{
    class ConfirmModalEngine : IStep<ForcedRecoveryStepState>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref ForcedRecoveryStepState token, int condition)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            SetModalData(token, modal);
            modalService.DisplayModal(modal);
        }

        private void SetModalData(ForcedRecoveryStepState token, ModalEV modal)
        {
            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modalToChange.Type.Type = ModalType.CONFIRM;
                    modalToChange.Confirm.PieceMoved = token.pieceMoved;
                    modalToChange.Confirm.PieceCaptured = token.pieceCaptured;
                });
        }
    }
}
