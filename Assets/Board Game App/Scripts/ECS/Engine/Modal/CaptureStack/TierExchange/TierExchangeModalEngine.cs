using Data.Enums.Modal;
using Data.Step.Piece.Ability.TierExchange;
using ECS.EntityView.Modal;
using Service.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal.CaptureStack.TierExchange
{
    class TierExchangeModalEngine : IStep<TierExchangeStepState>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref TierExchangeStepState token, int condition)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            SetModalOptions(modal, ref token);
            modalService.DisplayModal(modal);
        }

        private void SetModalOptions(ModalEV modal, ref TierExchangeStepState token)
        {
            int tileReferenceId = token.ReferenceTile.ID.entityID;

            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modal.Type.Type = ModalType.TIER_1_3_EXCHANGE_CLICK;
                    modal.CaptureOrStack.TileReferenceId = tileReferenceId;
                });
        }
    }
}
