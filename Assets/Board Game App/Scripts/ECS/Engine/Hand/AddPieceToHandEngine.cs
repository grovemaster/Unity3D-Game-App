using Data.Step.Piece.Ability.ForcedRearrangement;
using Data.Step.Piece.Capture;
using Service.Hand;
using Svelto.ECS;

namespace ECS.Engine.Hand
{
    class AddPieceToHandEngine : IStep<CapturePieceStepState>, IStep<ImmobileCapturePieceStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();

        private readonly ISequencer forcedRearrangementSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public AddPieceToHandEngine(ISequencer forcedRearrangementSequence)
        {
            this.forcedRearrangementSequence = forcedRearrangementSequence;
        }

        public void Ready()
        { }

        public void Step(ref CapturePieceStepState token, int condition)
        {
            handService.AddPieceToHand(token.PieceToCapture, entitiesDB);
        }

        public void Step(ref ImmobileCapturePieceStepState token, int condition)
        {
            handService.AddPieceToHand(token.PieceToCapture, entitiesDB);
            NextAction(ref token);
        }

        private void NextAction(ref ImmobileCapturePieceStepState token)
        {
            var forcedRearrangementToken = new ForcedRearrangementStepState
            {
                PieceToRearrange = token.PieceToCapture
            };

            forcedRearrangementSequence.Next(this, ref forcedRearrangementToken);
        }
    }
}
