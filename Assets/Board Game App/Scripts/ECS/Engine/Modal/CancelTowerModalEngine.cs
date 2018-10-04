using Data.Enums.AB;
using Data.Enums.Modal;
using Data.Step.Modal;
using ECS.EntityView.Modal;
using Service.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal
{
    class CancelTowerModalEngine : SingleEntityEngine<TowerModalEV>, IQueryingEntitiesEngine
    {
        private TowerModalService towerModalService = new TowerModalService();

        private readonly ISequencer cancelModalSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public CancelTowerModalEngine(ISequencer cancelModalSequence)
        {
            this.cancelModalSequence = cancelModalSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref TowerModalEV entityView)
        {
            entityView.Cancel.Cancel.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref TowerModalEV entityView)
        {
            entityView.Cancel.Cancel.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, bool cancel)
        {
            TowerModalEV modal = towerModalService.FindModalEV(entitiesDB);
            bool continueCancelling = modal.Type.Type != ModalType.CONFIRM
                || modal.Type.Type != ModalType.TOWER_3RD_TIER;

            var token = new CancelModalStepState();
            cancelModalSequence.Next(this, ref token, (int)(continueCancelling ? StepAB.A : StepAB.B));
        }
    }
}
