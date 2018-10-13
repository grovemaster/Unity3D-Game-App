using Data.Enums;
using Data.Enums.AB;
using Data.Enums.Click;
using Data.Enums.Modal;
using Data.Enums.Move;
using Data.Enums.Piece.PostMove;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Context.EngineStep.Create
{
    public class SetupSequence
    {
#pragma warning disable IDE0044 // Add readonly modifier
        private Dictionary<string, Sequencer> sequences;
        private Dictionary<string, IStep[]> steps;
        private Dictionary<string, IEngine> engines;
#pragma warning restore IDE0044 // Add readonly modifier

        public SetupSequence(
            Dictionary<string, Sequencer> sequences,
            Dictionary<string, IStep[]> steps,
            Dictionary<string, IEngine> engines)
        {
            this.sequences = sequences;
            this.steps = steps;
            this.engines = engines;
        }

        public void CreateSequences()
        {
            sequences.Add("boardPress", new Sequencer());
            sequences.Add("handPiecePress", new Sequencer());
            sequences.Add("cancelModal", new Sequencer());
            sequences.Add("cancelTowerModal", new Sequencer());
            sequences.Add("towerModalAnswer", new Sequencer());
            sequences.Add("captureStackModalAnswer", new Sequencer());
            sequences.Add("confirmModalAnswer", new Sequencer());
            sequences.Add("dropModalAnswer", new Sequencer());
        }

        public void SetSequences()
        {
            SetBoardPressSequence();
            SetSequenceHandPress();
            SetSequenceCancelModal();
            SetSequenceCancelTowerModal();
            SetSequenceTowerModalAnswer();
            SetSequenceCaptureStackModal();
            SetSequenceConfirmModal();
            SetSequenceDropFrontBackModal();
        }

        private void SetBoardPressSequence()
        {
            sequences["boardPress"].SetSequence(
                new Steps
                {
                    { // first step
                        engines["piecePress"],
                        new To
                        {
                            steps["press"]
                        }
                    },
                    { // also first step
                        engines["tilePress"],
                        new To
                        {
                            steps["press"]
                        }
                    },
                    {   // Clicking on board results in...
                        engines["boardPress"],
                        new To
                        {   // Highlight piece and tile(s)
                            { (int)BoardPress.CLICK_HIGHLIGHT, steps["determineClickType"] },
                            // Move piece or capture piece
                            { (int)BoardPress.MOVE_PIECE, steps["determineMoveType"] },
                            // Drop piece
                            { (int)BoardPress.DROP, steps["preDropAbilities"] },
                            // Substitution Modal
                            { (int)BoardPress.SUBSTITUTION, steps["substitutionModal"] },
                            // Tier Exchange Modal
                            { (int)BoardPress.TIER_1_3_EXCHANGE, steps["tierExchangeModal"] }
                        }
                    },
                    {
                        engines["determineClickType"],
                        new To
                        {
                            { (int)ClickState.CLICK_HIGHLIGHT, steps["click"] },
                            { (int)ClickState.TOWER_MODAL, steps["towerModal"] }
                        }
                    },
                    {   // Turn Start
                        engines["turnEnd"],
                        new To
                        {
                            steps["highlightAllDestinationTiles"]
                        }
                    },
                    {
                        engines["determineMoveType"],
                        new To
                        {
                            { (int)MoveState.MOVE_PIECE, steps["movePiece"] },
                            { (int)MoveState.MOBILE_CAPTURE, steps["capturePiece"] },
                            { (int)MoveState.CAPTURE_STACK_MODAL, steps["captureStackModal"] }
                        }
                    },
                    {
                        // After capturing opponent piece, still need to move turn piece
                        engines["gotoMovePiece"],
                        new To
                        {
                            steps["movePiece"]
                        }
                    },
                    {
                        engines["dropCheckStatus"],
                        new To
                        {
                            { (int)StepAB.A, steps["dropModal"] },
                            { (int)StepAB.B, steps["drop"] }
                        }
                    },
                    {
                        engines["preDropAbilities"],
                        new To
                        {
                            { (int)StepAB.A, steps["dropCheckStatusPrep"] },
                            { (int)StepAB.B, steps["dropCheckStatus"] }
                        }
                    },
                    {
                        engines["movePiece"],
                        new To
                        {
                            steps["determinePostMoveAction"]
                        }
                    },
                    {
                        engines["determinePostMoveAction"],
                        new To
                        {
                            // TODO Instead of generic enum, use a specifically-name enum { for rec, for rea, tur end }
                            { (int)PostMoveState.BETRAYAL, steps["betrayal"] },
                            { (int)PostMoveState.FORCED_RECOVERY, steps["forcedRecoveryCheck"] },
                            { (int)PostMoveState.FORCED_REARRANGEMENT, steps["forcedRearrangementCheck"] },
                            { (int)PostMoveState.TURN_END, steps["turnEnd"] }
                        }
                    },
                    {
                        engines["drop"],
                        new To
                        {
                            steps["turnEnd"]
                        }
                    },
                    {
                        engines["forcedRecoveryCheck"],
                        new To
                        {
                            { (int)StepAB.A, steps["confirmModal"] },
                            { (int)StepAB.B, steps["gotoForcedRearrangement"] }
                        }
                    },
                    {
                        engines["forcedRecoveryAbility"],
                        new To
                        {
                            steps["turnEnd"]
                        }
                    },
                    {
                        engines["addPieceToHand"],
                        new To
                        {
                            { (int)PostMoveState.BETRAYAL, steps["betrayal"] },
                            { (int)PostMoveState.FORCED_REARRANGEMENT, steps["forcedRearrangementCheck"] }
                        }
                    },
                    {
                        engines["gotoForcedRearrangement"],
                        new To
                        {
                            steps["forcedRearrangementCheck"]
                        }
                    },
                    {
                        engines["forcedRearrangementCheck"],
                        new To
                        {
                            { (int)StepAB.A, steps["forcedRearrangementAbility"] },
                            { (int)StepAB.B, steps["gotoTurnEndForcedRearrangementStepState"] }
                        }
                    },
                    {
                        engines["betrayal"],
                        new To
                        {
                            steps["forcedRearrangementCheck"]
                        }
                    },
                    {
                        engines["substitution"],
                        new To
                        {
                            steps["turnEnd"]
                        }
                    },
                    {
                        engines["tierExchange"],
                        new To
                        {
                            steps["turnEnd"]
                        }
                    },
                    {
                        engines["checkmate"],
                        new To
                        {
                            steps["checkmateModal"]
                        }
                    },
                    {
                        engines["decideClickImmobileCapture"],
                        new To
                        {
                            { (int)StepAB.A, steps["clickImmobileCaptureModal"] },
                            { (int)StepAB.B, steps["click"] }
                        }
                    }
                });
        }

        private void SetSequenceHandPress()
        {
            sequences["handPiecePress"].SetSequence(
                new Steps
                {
                    { // first step
                        engines["handPiecePress"],
                        new To
                        {
                            steps["handHighlight"]
                        }
                    }
                });
        }

        private void SetSequenceCancelModal()
        {
            sequences["cancelModal"].SetSequence(
                new Steps
                {
                    {
                        engines["cancelModal"],
                        new To
                        {
                            { (int)StepAB.A, steps["deHighlight"] },
                            { (int)StepAB.B, steps["gotoTurnEndCancelModalStepState"] }
                        }
                    }
                });
        }

        private void SetSequenceCancelTowerModal()
        {
            sequences["cancelTowerModal"].SetSequence(
                new Steps
                {
                    {
                        engines["cancelTowerModal"],
                        new To
                        {
                            { (int)StepAB.A, steps["deHighlight"] },
                            { (int)StepAB.B, steps["gotoTurnEndCancelModalStepState"] }
                        }
                    }
                });
        }

        public void SetSequenceTowerModalAnswer()
        {
            sequences["towerModalAnswer"].SetSequence(
                new Steps
                {
                    {
                        engines["towerModalAnswer"],
                        new To
                        {
                            { (int)TowerAnswerState.DECIDE_CLICK_HIGHLIGHT_CAPTURE, steps["decideClickImmobileCapture"] },
                            { (int)TowerAnswerState.DESIGNATE_IMMOBILE_CAPTURE, steps["designateImmobileCapture"]},
                            { (int)TowerAnswerState.INITIATE_IMMOBILE_CAPTURE, steps["immobileCapture"]}
                        }
                    },
                    {
                        engines["gotoTurnEnd"],
                        new To
                        {
                            steps["turnEnd"]
                        }
                    }
                });
        }

        private void SetSequenceCaptureStackModal()
        {
            sequences["captureStackModalAnswer"].SetSequence(
                new Steps
                {
                    {
                        engines["captureStackModalAnswer"],
                        new To
                        {
                            { (int)MoveState.MOVE_PIECE, steps["movePiece"] },
                            { (int)MoveState.MOBILE_CAPTURE, steps["capturePiece"] },
                            { (int)MoveState.SUBSTITUTION, steps["substitution"] },
                            { (int)MoveState.TIER_1_3_EXCHANGE, steps["tierExchange"] },
                            { (int)MoveState.CLICK, steps["click"] },
                            { (int)MoveState.IMMOBILE_CAPTURE, steps["immobileCapture"] }
                        }
                    }
                });
        }

        private void SetSequenceConfirmModal()
        {
            sequences["confirmModalAnswer"].SetSequence(
                new Steps
                {
                    {
                        engines["confirmModalAnswer"],
                        new To
                        {
                            { (int)StepAB.A, steps["forcedRecoveryAbility"] },
                            { (int)StepAB.B, steps["gotoForcedRearrangement"] }
                        }
                    }
                });
        }

        private void SetSequenceDropFrontBackModal()
        {
            sequences["dropModalAnswer"].SetSequence(
                new Steps
                {
                    {
                        engines["dropModalAnswer"],
                        new To
                        {
                            steps["drop"]
                        }
                    }
                });
        }
    }
}
