using Data.Step.Modal;
using Data.Step.Piece.Ability.ForcedRearrangement;
using Data.Step.Piece.Capture;
using Data.Step.Piece.Move;
using Data.Step.Turn;
using Svelto.ECS;

namespace ECS.Engine.Piece.Capture
{
    class GotoTurnEndEngine :
        IStep<ImmobileCapturePieceStepState>,
        IStep<ForcedRecoveryStepState>,
        IStep<ForcedRearrangementStepState>,
        IStep<CancelModalStepState>,
        IQueryingEntitiesEngine
    {
        private Sequencer towerModalAnswerSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public GotoTurnEndEngine(Sequencer towerModalAnswerSequence)
        {
            this.towerModalAnswerSequence = towerModalAnswerSequence;
        }

        public void Ready()
        { }

        public void Step(ref ImmobileCapturePieceStepState token, int condition)
        {
            NextActionTurnEnd();
        }

        public void Step(ref ForcedRecoveryStepState token, int condition)
        {
            NextActionTurnEnd();
        }

        public void Step(ref ForcedRearrangementStepState token, int condition)
        {
            NextActionTurnEnd();
        }

        public void Step(ref CancelModalStepState token, int condition)
        {
            NextActionTurnEnd();
        }

        private void NextActionTurnEnd()
        {
            var turnEndToken = new TurnEndStepState();
            towerModalAnswerSequence.Next(this, ref turnEndToken);
        }
    }
}
