using Data.Enums.Piece.PostMove;
using Data.Step.Piece.Ability.Betrayal;
using Data.Step.Piece.Ability.ForcedRearrangement;
using Data.Step.Piece.Capture;
using Service.Hand;
using Svelto.ECS;

namespace ECS.Engine.Hand
{
    class AddPieceToHandEngine : IStep<CapturePieceStepState>, IStep<ImmobileCapturePieceStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();

        private readonly ISequencer postMoveActionSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public AddPieceToHandEngine(ISequencer postMoveActionSequence)
        {
            this.postMoveActionSequence = postMoveActionSequence;
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

            if (token.BetrayalPossible)
            {
                NextActionBetrayal(ref token);
            }
            else
            {
                NextActionForcedRearrangement(ref token);
            }
        }

        private void NextActionBetrayal(ref ImmobileCapturePieceStepState token)
        {
            var betrayalToken = new BetrayalStepState
            {
                PieceMoved = token.pieceToStrike,
                PieceCaptured = token.PieceToCapture
            };

            postMoveActionSequence.Next(this, ref betrayalToken, (int)PostMoveState.BETRAYAL);
        }

        private void NextActionForcedRearrangement(ref ImmobileCapturePieceStepState token)
        {
            var forcedRearrangementToken = new ForcedRearrangementStepState
            {
                PieceToRearrange = token.PieceToCapture
            };

            postMoveActionSequence.Next(this, ref forcedRearrangementToken, (int)PostMoveState.FORCED_REARRANGEMENT);
        }
    }
}
