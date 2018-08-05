using Data.Step.Piece.Capture;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;

namespace ECS.Engine.Piece.Capture
{
    class MobileCapturePieceEngine : IStep<CapturePieceStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref CapturePieceStepState token, int condition)
        {
            PieceEV pieceToCapture = token.pieceToCapture;
            PieceService.SetPieceLocationToHandLocation(pieceToCapture, entitiesDB);
            pieceToCapture.Visibility.IsVisible.value = false;
        }
    }
}
