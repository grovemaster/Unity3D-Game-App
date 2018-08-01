using Data.Enum.Move;
using Data.Step.Piece.Capture;
using Data.Step.Piece.Move;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;
using System;

namespace ECS.Engine.Move
{
    class DetermineMoveTypeEngine : IStep<MovePieceStepState>, IQueryingEntitiesEngine
    {
        private readonly ISequencer moveSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public DetermineMoveTypeEngine(ISequencer moveSequence)
        {
            this.moveSequence = moveSequence;
        }

        public void Ready()
        { }

        public void Step(ref MovePieceStepState token, int condition)
        {
            PieceEV? topPieceAtDestinationTile = PieceService.FindTopPieceByLocation(
                   token.destinationTile.location.Location, entitiesDB);
            MoveState nextAction = DetermineMoveAction(ref token, topPieceAtDestinationTile);
            PerformNextAction(nextAction, ref token, topPieceAtDestinationTile);
        }

        private MoveState DetermineMoveAction(ref MovePieceStepState token, PieceEV? topPieceAtDestinationTile)
        {
            MoveState returnValue = MoveState.MOVE_PIECE;

            if (topPieceAtDestinationTile.HasValue
                && topPieceAtDestinationTile.Value.playerOwner.PlayerColor != token.pieceToMove.playerOwner.PlayerColor)
            {
                returnValue = MoveState.MOBILE_CAPTURE;
            }

            return returnValue;
        }

        private void PerformNextAction(
            MoveState nextAction, ref MovePieceStepState token, PieceEV? topPieceAtDestinationTile)
        {
            switch (nextAction)
            {
                case MoveState.MOVE_PIECE:
                    NextActionMovePiece(ref token);
                    break;
                case MoveState.MOBILE_CAPTURE:
                    NextActionMobileCapturePiece(ref token, topPieceAtDestinationTile.Value);
                    break;
                default:
                    throw new InvalidOperationException("Invalid or unsupported MoveState state");
            }
        }

        private void NextActionMovePiece(ref MovePieceStepState token)
        {
            moveSequence.Next(this, ref token, (int)MoveState.MOVE_PIECE);
        }

        private void NextActionMobileCapturePiece(ref MovePieceStepState token, PieceEV topPieceAtDestinationTile)
        {
            var captureToken = new CapturePieceStepState
            {
                pieceToCapture = topPieceAtDestinationTile,
                pieceToMove = token.pieceToMove,
                destinationTile = token.destinationTile
            };

            moveSequence.Next(this, ref captureToken, (int)MoveState.MOBILE_CAPTURE);
        }
    }
}
