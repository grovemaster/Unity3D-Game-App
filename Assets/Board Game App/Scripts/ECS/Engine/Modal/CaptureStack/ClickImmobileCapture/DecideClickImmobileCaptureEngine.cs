using System.Collections.Generic;
using Data.Enums;
using Data.Enums.AB;
using Data.Step;
using Data.Step.Modal;
using Data.Step.Piece.Click;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Board;
using Service.Board.Tile;
using Service.Piece.Find;
using Service.Piece.ImmobileCapture;
using Service.Turn;
using Svelto.ECS;

namespace ECS.Engine.Modal.CaptureStack.ClickImmobileCapture
{
    class DecideClickImmobileCaptureEngine : IStep<ClickPieceStepState>, IQueryingEntitiesEngine
    {
        private DestinationTileService destinationTileService = new DestinationTileService();
        private ImmobileCaptureService immobileCaptureService = new ImmobileCaptureService();
        private PieceFindService pieceFindService = new PieceFindService();
        private TileService tileService = new TileService();
        private TurnService turnService = new TurnService();

        private readonly ISequencer decideClickImmobileCaptureSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public DecideClickImmobileCaptureEngine(ISequencer decideClickImmobileCaptureSequence)
        {
            this.decideClickImmobileCaptureSequence = decideClickImmobileCaptureSequence;
        }

        public void Ready()
        { }

        public void Step(ref ClickPieceStepState token, int condition)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            List<PieceEV> piecesAtLocation = pieceFindService.FindPiecesByLocation(token.ClickedPiece.Location.Location, entitiesDB);

            if (currentTurn.TurnPlayer.PlayerColor == token.ClickedPiece.PlayerOwner.PlayerColor
                && currentTurn.TurnPlayer.PlayerColor != piecesAtLocation[piecesAtLocation.Count - 2].PlayerOwner.PlayerColor
                && IsImmobileCapturePossible(token.ClickedPiece, piecesAtLocation, currentTurn))
            {
                NextActionClickImmobileCaptureModal(ref token);
            }
            else
            {
                NextActionClickHighlight(ref token);
            }
        }

        private bool IsImmobileCapturePossible(PieceEV piece, List<PieceEV> piecesAtLocation, TurnEV currentTurn)
        {
            // TODO Immobile capture issues should only be a potential problem if the pieceToCapture has Mobile Range Expansion (since top piece is friendly)
            bool immobileCapturePossible = immobileCaptureService.ImmobileCapturePossible(piecesAtLocation, currentTurn.TurnPlayer.PlayerColor, entitiesDB);
            bool noCheckViolationsExist = immobileCaptureService.NoCheckViolationsExist(piecesAtLocation, immobileCapturePossible, entitiesDB);

            return immobileCapturePossible && noCheckViolationsExist;
        }

        private void NextActionClickImmobileCaptureModal(ref ClickPieceStepState token)
        {
            var clickState = new ClickImmobileCaptureStepState
            {
                TileReferenceEV = tileService.FindTileEV(token.ClickedPiece.Location.Location, entitiesDB)
            };

            decideClickImmobileCaptureSequence.Next(this, ref clickState, (int)StepAB.A);
        }

        private void NextActionClickHighlight(ref ClickPieceStepState token)
        {
            var pressState = new PressStepState
            {
                PieceEntityId = token.ClickedPiece.ID.entityID,
                PiecePressState = token.ClickedPiece.Highlight.IsHighlighted ? PiecePressState.UNCLICKED : PiecePressState.CLICKED,
                AffectedTiles = destinationTileService.CalcDestinationTileLocations(token.ClickedPiece, entitiesDB)
            };

            decideClickImmobileCaptureSequence.Next(this, ref pressState, (int)StepAB.B);
        }
    }
}
