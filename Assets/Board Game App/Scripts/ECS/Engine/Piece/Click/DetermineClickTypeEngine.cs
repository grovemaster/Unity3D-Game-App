using Data.Enums;
using Data.Enums.Click;
using Data.Step;
using Data.Step.Piece.Click;
using ECS.EntityView.Turn;
using Service.Board;
using Service.Turn;
using Svelto.ECS;
using System;

namespace ECS.Engine.Piece.Click
{
    class DetermineClickTypeEngine : IStep<ClickPieceStepState>, IQueryingEntitiesEngine
    {
        private DestinationTileService destinationTileService = new DestinationTileService();
        private TurnService turnService = new TurnService();

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
            return token.ClickedPiece.Tier.Tier > 1 ? ClickState.TOWER_MODAL : ClickState.CLICK_HIGHLIGHT;
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
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);

            // Determine PiecePressState, click or un-click, and destination tiles
            var pressState = new PressStepState
            {
                PieceEntityId = token.ClickedPiece.ID.entityID,
                PiecePressState = token.ClickedPiece.Highlight.IsHighlighted ? PiecePressState.UNCLICKED : PiecePressState.CLICKED,
                AffectedTiles = destinationTileService.CalcDestinationTileLocations(
                    token.ClickedPiece,
                    entitiesDB,
                    null,
                    currentTurn.TurnPlayer.PlayerColor == token.ClickedPiece.PlayerOwner.PlayerColor)
            };

            clickSequence.Next(this, ref pressState, (int)ClickState.CLICK_HIGHLIGHT);
        }

        private void NextActionTowerModal(ref ClickPieceStepState token)
        {
            clickSequence.Next(this, ref token, (int)ClickState.TOWER_MODAL);
        }
    }
}
