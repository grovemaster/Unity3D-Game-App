using Data.Enums.Move;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using Data.Piece.Map;
using Data.Step.Piece.Capture;
using Data.Step.Piece.Move;
using ECS.EntityView.Piece;
using Service.Move;
using Service.Piece.Find;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECS.Engine.Move
{
    class DetermineMoveTypeEngine : IStep<MovePieceStepState>, IQueryingEntitiesEngine
    {
        private DetermineMoveTypeService determineMoveTypeService = new DetermineMoveTypeService();
        private PieceFindService pieceFindService = new PieceFindService();

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
            PieceEV? topPieceAtDestinationTile = pieceFindService.FindTopPieceByLocation(
                   token.DestinationTile.Location.Location, entitiesDB);
            MoveState nextAction = DetermineMoveAction(ref token, topPieceAtDestinationTile);
            PerformNextAction(nextAction, ref token, topPieceAtDestinationTile);
        }

        private MoveState DetermineMoveAction(ref MovePieceStepState token, PieceEV? topPieceAtDestinationTile)
        {
            MoveState returnValue = MoveState.MOVE_PIECE;

            if (topPieceAtDestinationTile.HasValue
                && topPieceAtDestinationTile.Value.PlayerOwner.PlayerColor != token.PieceToMove.PlayerOwner.PlayerColor
                && !TwoFileMoveViolatedByBetrayal(ref token))
            {
                bool isMobileStackValid = topPieceAtDestinationTile.Value.Tier.Tier != 3 && determineMoveTypeService.IsMobileStackValid(token.PieceToMove, token.DestinationTile.Location.Location, entitiesDB);
                bool isMobileCaptureValid = determineMoveTypeService.IsMobileCaptureValid(token.PieceToMove, token.DestinationTile.Location.Location, entitiesDB);

                if (isMobileStackValid && isMobileCaptureValid)
                {
                    returnValue = MoveState.CAPTURE_STACK_MODAL;
                }
                else if (isMobileCaptureValid)
                {
                    returnValue = MoveState.MOBILE_CAPTURE;
                }
                // Else it's MoveState.MOVE_PIECE (stack), the default value
            }

            return returnValue;
        }

        private bool TwoFileMoveViolatedByBetrayal(ref MovePieceStepState token)
        {
            List<PieceEV> towerPieces = pieceFindService.FindPiecesByLocation(token.DestinationTile.Location.Location, entitiesDB);
            PieceEV pieceToMove = token.PieceToMove;

            return towerPieces.Count > 0
                && AbilityToPiece.HasAbility(PreMoveAbility.TWO_FILE_MOVE, token.PieceToMove.Piece.PieceType)
                && AbilityToPiece.HasAbility(PostMoveAbility.BETRAYAL, token.PieceToMove.Piece.PieceType)
                && towerPieces.Where(piece => piece.Piece.PieceType == pieceToMove.Piece.Front
                    && !piece.Tier.TopOfTower).ToList().Count > 0;
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
                case MoveState.CAPTURE_STACK_MODAL:
                    NextActionMobileCaptureStackModal(ref token, topPieceAtDestinationTile.Value);
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
                PieceToCapture = topPieceAtDestinationTile,
                PieceToMove = token.PieceToMove,
                DestinationTile = token.DestinationTile
            };

            moveSequence.Next(this, ref captureToken, (int)MoveState.MOBILE_CAPTURE);
        }

        private void NextActionMobileCaptureStackModal(ref MovePieceStepState token, PieceEV topPieceAtDestinationTile)
        {
            var captureToken = new CapturePieceStepState
            {
                PieceToCapture = topPieceAtDestinationTile,
                PieceToMove = token.PieceToMove,
                DestinationTile = token.DestinationTile
            };

            moveSequence.Next(this, ref captureToken, (int)MoveState.CAPTURE_STACK_MODAL);
        }
    }
}
