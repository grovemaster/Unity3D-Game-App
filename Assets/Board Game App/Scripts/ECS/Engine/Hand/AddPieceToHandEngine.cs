using Data.Step.Piece.Capture;
using Service.Hand;
using Svelto.ECS;

namespace ECS.Engine.Hand
{
    class AddPieceToHandEngine : IStep<CapturePieceStepState>, IStep<ImmobileCapturePieceStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref CapturePieceStepState token, int condition)
        {
            handService.AddPieceToHand(token.PieceToCapture, entitiesDB);
        }

        public void Step(ref ImmobileCapturePieceStepState token, int condition)
        {
            handService.AddPieceToHand(token.PieceToCapture, entitiesDB);
        }
    }
}
