using Data.Enums.Modal;
using Data.Step.Piece.Ability.Substitution;
using Data.Step.Piece.Capture;
using ECS.EntityView.Modal;
using Service.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal.CaptureStack.Substitution
{
    class SubstitutionModalEngine : IStep<SubstitutionStepState>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref SubstitutionStepState token, int condition)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            SetModalOptions(modal, ref token);
            modalService.DisplayModal(modal);
        }

        private void SetModalOptions(ModalEV modal, ref SubstitutionStepState token)
        {
            int tileReferenceId = token.TileReferenceEV.ID.entityID;

            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modal.Type.Type = ModalType.SUBSTITUTION_CLICK;
                    modal.CaptureOrStack.TileReferenceId = tileReferenceId;
                });
        }
    }
}
