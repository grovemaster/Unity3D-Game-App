using Data.Step.Modal;
using ECS.EntityView.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal
{
    class CancelModalEngine : SingleEntityEngine<ModalEV>, IQueryingEntitiesEngine
    {
        private readonly ISequencer cancelModalSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public CancelModalEngine(ISequencer cancelModalSequence)
        {
            this.cancelModalSequence = cancelModalSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref ModalEV entityView)
        {
            entityView.Cancel.Cancel.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref ModalEV entityView)
        {
            entityView.Cancel.Cancel.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, bool cancel)
        {
            var token = new CancelModalStepState();
            cancelModalSequence.Next(this, ref token);
        }
    }
}
