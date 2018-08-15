using Data.Step;
using Data.Step.Board;
using Data.Step.Drop;
using Data.Step.Hand;
using Data.Step.Modal;
using Data.Step.Piece.Capture;
using Data.Step.Piece.Click;
using Data.Step.Piece.Move;
using Data.Step.Turn;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Context.EngineStep.Create
{
    public class SetupStep
    {
        private Dictionary<string, IStep[]> steps;
        private Dictionary<string, IEngine> engines;

        public SetupStep(
            Dictionary<string, IStep[]> steps,
            Dictionary<string, IEngine> engines)
        {
            this.steps = steps;
            this.engines = engines;
        }

        public void Create()
        {
            #region Press & Click
            steps.Add("press", new IStep<BoardPressStepState>[]
            {
                (IStep<BoardPressStepState>)engines["unPress"],
                (IStep<BoardPressStepState>)engines["boardPress"]
            });

            steps.Add("click", new IStep<PressStepState>[]
            {
                (IStep<PressStepState>)engines["deHighlightTeamPieces"],
                (IStep<PressStepState>)engines["pieceHighlight"],
                (IStep<PressStepState>)engines["tileHighlight"]
            });
            #endregion

            #region Determine
            steps.Add("determineClickType", new IStep<ClickPieceStepState>[]
            {
                (IStep<ClickPieceStepState>)engines["determineClickType"]
            });

            steps.Add("determineMoveType", new IStep<MovePieceStepState>[]
            {
                (IStep<MovePieceStepState>)engines["determineMoveType"]
            });
            #endregion

            #region Abilities
            steps.Add("preDropAbilities", new IStep<DropStepState>[]
            {
                (IStep<DropStepState>)engines["preDropAbilities"]
            });
            #endregion

            #region Modal
            steps.Add("towerModal", new IStep<ClickPieceStepState>[]
            {
                (IStep<ClickPieceStepState>)engines["towerModal"]
            });

            steps.Add("captureStackModal", new IStep<CapturePieceStepState>[]
            {
                (IStep<CapturePieceStepState>)engines["captureStackModal"]
            });
            #endregion

            #region Highlight
            steps.Add("highlightAllDestinationTiles", new IStep<TurnStartStepState>[]
            {
                (IStep<TurnStartStepState>)engines["highlightAllDestinationTiles"]
            });

            steps.Add("handHighlight", new IStep<HandPiecePressStepState>[]
            {
                (IStep<HandPiecePressStepState>)engines["deHighlightTeamPieces"],
                (IStep<HandPiecePressStepState>)engines["handPieceHighlight"]
            });

            steps.Add("deHighlight", new IStep<CancelModalStepState>[]
            {
                (IStep<CancelModalStepState>)engines["deHighlightTeamPieces"]
            });
            #endregion


            #region Drop
            steps.Add("drop", new IStep<DropStepState>[]
            {
                (IStep<DropStepState>)engines["drop"],
                (IStep<DropStepState>)engines["unHighlight"],
                (IStep<DropStepState>)engines["turnEnd"]
            });
            #endregion

            #region Move
            steps.Add("movePiece", new IStep<MovePieceStepState>[]
            {
                (IStep<MovePieceStepState>)engines["movePiece"],
                (IStep<MovePieceStepState>)engines["unHighlight"],
                (IStep<MovePieceStepState>)engines["movePieceCleanup"],
                (IStep<MovePieceStepState>)engines["turnEnd"]
            });
            #endregion

            #region Turn End
            steps.Add("movePieceCleanup", new IStep<MovePieceStepState>[]
            {
                (IStep<MovePieceStepState>)engines["unHighlight"],
                (IStep<MovePieceStepState>)engines["movePieceCleanup"],
                (IStep<MovePieceStepState>)engines["turnEnd"]
            });
            #endregion

            #region Capture
            steps.Add("capturePiece", new IStep<CapturePieceStepState>[]
            {
                (IStep<CapturePieceStepState>)engines["mobileCapturePiece"],
                (IStep<CapturePieceStepState>)engines["addPieceToHand"],
                (IStep<CapturePieceStepState>)engines["gotoMovePiece"]
            });

            steps.Add("designateImmobileCapture", new IStep<ImmobileCaptureStepState>[]
            {
                (IStep<ImmobileCaptureStepState>)engines["designateImmobileCapture"]
            });

            steps.Add("immobileCapture", new IStep<ImmobileCapturePieceStepState>[]
            {
                (IStep<ImmobileCapturePieceStepState>)engines["immobileCapture"],
                (IStep<ImmobileCapturePieceStepState>)engines["addPieceToHand"],
                (IStep<ImmobileCapturePieceStepState>)engines["gotoMovePieceCleanup"]
            });
            #endregion
        }
    }
}
