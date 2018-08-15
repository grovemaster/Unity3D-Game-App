using Data.Step.Piece.Capture;
using Data.Step.Turn;
using Svelto.ECS;

namespace ECS.Engine.Piece.Capture
{
    class GotoTurnEndEngine : IStep<ImmobileCapturePieceStepState>, IQueryingEntitiesEngine
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
            var turnEndToken = new TurnEndStepState();
            towerModalAnswerSequence.Next(this, ref turnEndToken);
        }
    }
}
