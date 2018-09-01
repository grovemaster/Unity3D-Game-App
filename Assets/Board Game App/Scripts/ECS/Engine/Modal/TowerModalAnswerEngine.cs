using Data.Enums;
using Data.Enums.Modal;
using Data.Step;
using Data.Step.Modal;
using Data.Step.Piece.Capture;
using ECS.EntityView.Modal;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Board;
using Service.Modal;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;
using System;
using System.Collections.Generic;

namespace ECS.Engine.Modal
{
    class TowerModalAnswerEngine : SingleEntityEngine<ModalEV>, IQueryingEntitiesEngine
    {
        private DestinationTileService destinationTileService = new DestinationTileService();
        private ModalService modalService = new ModalService();
        private PieceFindService pieceFindService = new PieceFindService();
        private TurnService turnService = new TurnService();

        private readonly ISequencer towerModalConfirmSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public TowerModalAnswerEngine(ISequencer towerModalConfirmSequence)
        {
            this.towerModalConfirmSequence = towerModalConfirmSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref ModalEV entityView)
        {
            entityView.Answer.Answer.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref ModalEV entityView)
        {
            entityView.Answer.Answer.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, int pieceReferenceId)
        {
            PieceEV piece = FindAssociatedPiece(pieceReferenceId);
            TowerAnswerState nextAction = DecideNextAction(piece);
            PerformNextAction(nextAction, piece);
        }

        private PieceEV FindAssociatedPiece(int pieceId)
        {
            return pieceFindService.FindPieceEV(pieceId, entitiesDB);
        }

        private TowerAnswerState DecideNextAction(PieceEV piece)
        {
            TowerAnswerState returnValue = TowerAnswerState.CLICK_HIGHLIGHT;
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            
            if (!piece.Tier.TopOfTower)
            {
                returnValue = TowerAnswerState.DESIGNATE_IMMOBILE_CAPTURE;
            }
            // It's possible to click non-topOfTower piece while immobile capture designated, hence the override
            if (modal.ImmobileCaptureState.ImmobileCaptureDesignated)
            {
                returnValue = TowerAnswerState.INITIATE_IMMOBILE_CAPTURE;
            }

            return returnValue;
        }

        private void PerformNextAction(TowerAnswerState nextAction, PieceEV piece)
        {
            switch(nextAction)
            {
                case TowerAnswerState.CLICK_HIGHLIGHT:
                    NextActionClickHighlight(piece);
                    break;
                case TowerAnswerState.DESIGNATE_IMMOBILE_CAPTURE:
                    NextActionDesignateImmobileCapture();
                    break;
                case TowerAnswerState.INITIATE_IMMOBILE_CAPTURE:
                    NextActionInitiateImmobileCapture(piece);
                    break;
                default:
                    throw new InvalidOperationException("Invalid or unsupported TowerAnswer state");
            }
        }

        private void NextActionClickHighlight(PieceEV piece)
        {
            var pressState = new PressStepState
            {
                PieceEntityId = piece.ID.entityID,
                PiecePressState = piece.Highlight.IsHighlighted ? PiecePressState.UNCLICKED : PiecePressState.CLICKED,
                AffectedTiles = destinationTileService.CalcDestinationTileLocations(piece, entitiesDB)
            };

            towerModalConfirmSequence.Next(this, ref pressState, (int)TowerAnswerState.CLICK_HIGHLIGHT);
        }

        private void NextActionDesignateImmobileCapture()
        {
            var immobileCaptureStepState = new ImmobileCaptureStepState();

            towerModalConfirmSequence.Next(this, ref immobileCaptureStepState, (int)TowerAnswerState.DESIGNATE_IMMOBILE_CAPTURE);
        }

        private void NextActionInitiateImmobileCapture(PieceEV piece)
        {
            /**
             * Scenarios, [Tier1, Tier2, Tier3], F=Friendly, E=Enemy:
             * * FO, FOO, OFO
             * * OF, OFF, FOF
             * 
             * The pieceToCapture is either the piece param (piece that was clicked) or an
             * adjacent piece.  Business logic:
             * 
             * If topOfTower is NOT turn player color, then it's piece that was clicked
             * Else it's the adjacent piece that is NOT turn player color; there will only
             * be one of those
             */

            PieceEV pieceToCapture = piece;
            PieceEV topOfTowerPiece = pieceFindService.FindTopPieceByLocation(piece.Location.Location, entitiesDB).Value;
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);

            //if (topOfTowerPiece.PlayerOwner.PlayerColor != currentTurn.TurnPlayer.PlayerColor)
            //{
            //    pieceToCapture = piece;
            //}
            //else
            if (topOfTowerPiece.PlayerOwner.PlayerColor == currentTurn.TurnPlayer.PlayerColor)
            {
                List<PieceEV> pieces = pieceFindService.FindPiecesByLocation(piece.Location.Location, entitiesDB);
                bool pieceFound = false;

                foreach (PieceEV pieceToCheck in pieces)
                {
                    if (Math.Abs(piece.Tier.Tier - pieceToCheck.Tier.Tier) == 1
                        && currentTurn.TurnPlayer.PlayerColor != pieceToCheck.PlayerOwner.PlayerColor)
                    {
                        pieceToCapture = pieceToCheck;
                        pieceFound = true;
                        break;
                    }
                }

                if (!pieceFound)
                {
                    throw new InvalidOperationException("Did not find adjacent piece of opposite color.");
                }
            }

            var immobileCapturePieceStepState = new ImmobileCapturePieceStepState
            {
                PieceToCapture = pieceToCapture
            };

            towerModalConfirmSequence.Next(this, ref immobileCapturePieceStepState, (int)TowerAnswerState.INITIATE_IMMOBILE_CAPTURE);
        }
    }
}
