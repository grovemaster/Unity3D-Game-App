using Data.Step;
using Data.Step.Board;
using Data.Step.Drop;
using Data.Step.Hand;
using Data.Step.Modal;
using Data.Step.Piece.Ability;
using Data.Step.Piece.Ability.Betrayal;
using Data.Step.Piece.Ability.ForcedRearrangement;
using Data.Step.Piece.Ability.Substitution;
using Data.Step.Piece.Ability.TierExchange;
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
            steps.Add("preDropAbilities", new IStep<DropPrepStepState>[]
            {
                (IStep<DropPrepStepState>)engines["preDropAbilities"]
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

            steps.Add("substitutionModal", new IStep<SubstitutionStepState>[]
            {
                (IStep<SubstitutionStepState>)engines["substitutionModal"]
            });

            steps.Add("tierExchangeModal", new IStep<TierExchangeStepState>[]
            {
                (IStep<TierExchangeStepState>)engines["tierExchangeModal"]
            });

            steps.Add("confirmModal", new IStep<ForcedRecoveryStepState>[]
            {
                (IStep<ForcedRecoveryStepState>)engines["confirmModal"]
            });

            #region Drop Modal
            steps.Add("dropModal", new IStep<DropPrepStepState>[]
            {
                (IStep<DropPrepStepState>)engines["dropModal"]
            });
            #endregion
            #endregion

            #region Highlight
            steps.Add("highlightAllDestinationTiles", new IStep<TurnStartStepState>[]
            {
                (IStep<TurnStartStepState>)engines["commanderCheck"],
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
                (IStep<DropStepState>)engines["drop"]
            });

            steps.Add("dropCheckStatusPrep", new IStep<DropPrepStepState>[]
            {
                (IStep<DropPrepStepState>)engines["dropCheckStatus"]
            });

            steps.Add("dropCheckStatus", new IStep<DropStepState>[]
            {
                (IStep<DropStepState>)engines["dropCheckStatus"]
            });
            #endregion

            #region Determine Post Move Action
            steps.Add("determinePostMoveAction", new IStep<DeterminePostMoveStepState>[]
            {
                (IStep<DeterminePostMoveStepState>)engines["determinePostMoveAction"]
            });
            #endregion

            #region Forced Recovery
            steps.Add("forcedRecoveryCheck", new IStep<ForcedRecoveryStepState>[]
            {
                (IStep<ForcedRecoveryStepState>)engines["forcedRecoveryCheck"]
            });

            steps.Add("forcedRecoveryAbility", new IStep<ForcedRecoveryStepState>[]
            {
                (IStep<ForcedRecoveryStepState>)engines["forcedRecoveryAbility"]
            });
            #endregion

            #region Forced Rearrangement
            steps.Add("forcedRearrangementCheck", new IStep<ForcedRearrangementStepState>[]
            {
                (IStep<ForcedRearrangementStepState>)engines["forcedRearrangementCheck"]
            });

            steps.Add("forcedRearrangementAbility", new IStep<ForcedRearrangementStepState>[]
            {
                (IStep<ForcedRearrangementStepState>)engines["forcedRearrangementAbility"]
            });

            steps.Add("gotoForcedRearrangement", new IStep<ForcedRecoveryStepState>[]
            {
                (IStep<ForcedRecoveryStepState>)engines["gotoForcedRearrangement"]
            });
            #endregion

            #region Substitution
            steps.Add("substitution", new IStep<SubstitutionStepState>[]
            {
                (IStep<SubstitutionStepState>)engines["substitution"]
            });
            #endregion

            #region Tier Exchange
            steps.Add("tierExchange", new IStep<TierExchangeStepState>[]
            {
                (IStep<TierExchangeStepState>)engines["tierExchange"]
            });
            #endregion

            #region Betrayal
            steps.Add("betrayal", new IStep<BetrayalStepState>[]
            {
                (IStep<BetrayalStepState>)engines["betrayal"]
            });
            #endregion

            #region Move
            steps.Add("movePiece", new IStep<MovePieceStepState>[]
            {
                (IStep<MovePieceStepState>)engines["movePiece"]
            });
            #endregion

            #region Turn End
            steps.Add("turnEnd", new IStep<TurnEndStepState>[]
            {
                (IStep<TurnEndStepState>)engines["unHighlight"],
                (IStep<TurnEndStepState>)engines["movePieceCleanup"],
                (IStep<TurnEndStepState>)engines["turnEnd"]
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
                (IStep<ImmobileCapturePieceStepState>)engines["addPieceToHand"]
            });
            #endregion

            #region Goto Turn End
            steps.Add("gotoTurnEndForcedRearrangementStepState", new IStep<ForcedRearrangementStepState>[]
            {
                (IStep<ForcedRearrangementStepState>)engines["gotoTurnEnd"]
            });
            
            steps.Add("gotoTurnEndForcedRecoveryStepState", new IStep<ForcedRecoveryStepState>[]
            {
                (IStep<ForcedRecoveryStepState>)engines["gotoTurnEnd"]
            });

            steps.Add("gotoTurnEndCancelModalStepState", new IStep<CancelModalStepState>[]
            {
                (IStep<CancelModalStepState>)engines["gotoTurnEnd"]
            });
            #endregion
        }
    }
}
