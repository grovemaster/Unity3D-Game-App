using Data.Step.Hand;
using ECS.EntityView.Hand;
using Service.Hand;
using Svelto.ECS;

namespace ECS.Engine.Hand.Highlight
{
    class HandPieceHighlightEngine : IStep<HandPiecePressStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();

        public HandPieceHighlightEngine()
        { }

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref HandPiecePressStepState token, int condition)
        {
            HandPieceEV handPiece = handService.FindHandPiece(token.HandPieceEntityId, entitiesDB);
            bool isClicked = !handPiece.Highlight.IsHighlighted;
            handService.HighlightHandPiece(ref handPiece, isClicked, entitiesDB);
        }
    }
}
