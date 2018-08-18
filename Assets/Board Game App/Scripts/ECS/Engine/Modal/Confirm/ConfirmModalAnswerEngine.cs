using System;
using Data.Enum.AB;
using Data.Step.Piece.Move;
using Data.Step.Turn;
using ECS.EntityView.Modal;
using Service.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal.Confirm
{
    class ConfirmModalAnswerEngine : SingleEntityEngine<ModalEV>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();
        private readonly ISequencer confirmModalConfirmSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public ConfirmModalAnswerEngine(ISequencer confirmModalConfirmSequence)
        {
            this.confirmModalConfirmSequence = confirmModalConfirmSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref ModalEV entityView)
        {
            entityView.Confirm.Answer.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref ModalEV entityView)
        {
            entityView.Confirm.Answer.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, bool answer)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            NextAction(modal, answer);
        }

        private void NextAction(ModalEV modal, bool answer)
        {
            var forcedRecoveryToken = new ForcedRecoveryStepState
            {
                pieceMoved = modal.Confirm.PieceMoved,
                pieceCaptured = modal.Confirm.PieceCaptured
            };

            confirmModalConfirmSequence.Next(this, ref forcedRecoveryToken, (int)(answer ? StepAB.A : StepAB.B));
        }
    }
}
