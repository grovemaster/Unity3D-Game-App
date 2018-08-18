using Data.Enum;
using Data.Enum.Click;
using Data.Step;
using Data.Step.Piece.Click;
using Service.Board;
using Svelto.ECS;
using System;

namespace ECS.Engine.Piece.Click
{
    class DetermineClickTypeEngine : IStep<ClickPieceStepState>, IQueryingEntitiesEngine
    {
        private DestinationTileService destinationTileService = new DestinationTileService();

        private readonly ISequencer clickSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public DetermineClickTypeEngine(ISequencer clickSequence)
        {
            this.clickSequence = clickSequence;
        }

        public void Ready()
        { }

        public void Step(ref ClickPieceStepState token, int condition)
        {
            ClickState nextAction = DetermineMoveAction(ref token);
            PerformNextAction(nextAction, ref token);
        }

        private ClickState DetermineMoveAction(ref ClickPieceStepState token)
        {
            return token.clickedPiece.Tier.Tier > 1 ? ClickState.TOWER_MODAL : ClickState.CLICK_HIGHLIGHT;
        }

        private void PerformNextAction(ClickState nextAction, ref ClickPieceStepState token)
        {
            switch (nextAction)
            {
                case ClickState.CLICK_HIGHLIGHT:
                    NextActionClickHighlight(ref token);
                    break;
                case ClickState.TOWER_MODAL:
                    NextActionTowerModal(ref token);
                    break;
                default:
                    throw new InvalidOperationException("Invalid or unsupported MoveState state");
            }
        }

        private void NextActionClickHighlight(ref ClickPieceStepState token)
        {
            // Determine PiecePressState, click or un-click, and destination tiles
            var pressState = new PressStepState
            {
                pieceEntityId = token.clickedPiece.ID.entityID,
                piecePressState = token.clickedPiece.Highlight.IsHighlighted ? PiecePressState.UNCLICKED : PiecePressState.CLICKED,
                affectedTiles = destinationTileService.CalcDestinationTileLocations(token.clickedPiece, entitiesDB)
            };

            clickSequence.Next(this, ref pressState, (int)ClickState.CLICK_HIGHLIGHT);
        }

        private void NextActionTowerModal(ref ClickPieceStepState token)
        {
            clickSequence.Next(this, ref token, (int)ClickState.TOWER_MODAL);
        }
    }
}
