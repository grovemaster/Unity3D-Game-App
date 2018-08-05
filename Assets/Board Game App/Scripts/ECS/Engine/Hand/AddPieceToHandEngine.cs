using Data.Step.Piece.Capture;
using ECS.EntityView.Hand;
using ECS.EntityView.Turn;
using Service.Hand;
using Service.Turn;
using Svelto.ECS;

namespace ECS.Engine.Hand
{
    class AddPieceToHandEngine : IStep<CapturePieceStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref CapturePieceStepState token, int condition)
        {
            TurnEV turnPlayer = TurnService.GetCurrentTurnEV(entitiesDB);
            HandPieceEV handHoldingCapturedPiece = handService.FindHandPiece(
                token.pieceToCapture.Piece.PieceType, turnPlayer.TurnPlayer.PlayerColor, entitiesDB);
            handHoldingCapturedPiece.HandPiece.NumPieces.value++;
            
        }
    }
}
