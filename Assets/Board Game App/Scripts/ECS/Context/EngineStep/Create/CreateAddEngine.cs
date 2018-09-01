using ECS.Engine.Board;
using ECS.Engine.Board.Tile;
using ECS.Engine.Board.Tile.Highlight;
using ECS.Engine.Check;
using ECS.Engine.Drop;
using ECS.Engine.Hand;
using ECS.Engine.Hand.Highlight;
using ECS.Engine.Modal;
using ECS.Engine.Modal.CaptureStack;
using ECS.Engine.Modal.Confirm;
using ECS.Engine.Modal.Drop;
using ECS.Engine.Modal.ImmobileCapture;
using ECS.Engine.Move;
using ECS.Engine.Piece;
using ECS.Engine.Piece.Ability.Betrayal;
using ECS.Engine.Piece.Ability.Determine;
using ECS.Engine.Piece.Ability.Drop;
using ECS.Engine.Piece.Ability.ForcedRearrangement;
using ECS.Engine.Piece.Ability.ForcedRecovery;
using ECS.Engine.Piece.Capture;
using ECS.Engine.Piece.Click;
using ECS.Engine.Piece.Move;
using ECS.Engine.Turn;
using Engine.Check.Drop;
using Engine.Piece.Ability.ForcedRearrangement.Goto;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Context.EngineStep.Create
{
    public class CreateAddEngine
    {
        private EnginesRoot enginesRoot;
        private Dictionary<string, IEngine> engines;
        private Dictionary<string, Sequencer> sequences;

        public CreateAddEngine(
            EnginesRoot enginesRoot,
            Dictionary<string, IEngine> engines,
            Dictionary<string, Sequencer> sequences)
        {
            this.enginesRoot = enginesRoot;
            this.engines = engines;
            this.sequences = sequences;
        }

        public void CreateEngines()
        {
            engines.Add("piecePress", new PiecePressEngine(sequences["boardPress"]));
            engines.Add("tilePress", new TilePressEngine(sequences["boardPress"]));
            engines.Add("unPress", new UnPressEngine());
            engines.Add("boardPress", new BoardPressEngine(sequences["boardPress"]));
            engines.Add("deHighlightTeamPieces", new DeHighlightTeamPiecesEngine());
            engines.Add("pieceHighlight", new PieceHighlightEngine());
            engines.Add("tileHighlight", new TileHighlightEngine());

            engines.Add("determineMoveType", new DetermineMoveTypeEngine(sequences["boardPress"]));

            engines.Add("unHighlight", new UnHighlightEngine());
            engines.Add("movePiece", new MovePieceEngine(sequences["boardPress"]));
            engines.Add("movePieceCleanup", new MovePieceCleanupEngine());
            engines.Add("turnEnd", new TurnEndEngine(sequences["boardPress"]));

            engines.Add("commanderCheck", new CommanderCheckEngine());
            engines.Add("highlightAllDestinationTiles", new HighlightAllDestinationTilesEngine());

            engines.Add("mobileCapturePiece", new MobileCapturePieceEngine());
            engines.Add("addPieceToHand", new AddPieceToHandEngine(sequences["boardPress"]));
            engines.Add("gotoMovePiece", new GotoMovePieceEngine(sequences["boardPress"]));

            engines.Add("handPiecePress", new HandPiecePressEngine(sequences["handPiecePress"]));
            engines.Add("handPieceHighlight", new HandPieceHighlightEngine());

            engines.Add("dropCheckStatus", new DropCheckStatusEngine(sequences["boardPress"]));
            engines.Add("preDropAbilities", new PreDropAbilitiesEngine(sequences["boardPress"]));
            engines.Add("drop", new DropEngine(sequences["boardPress"]));

            engines.Add("determinePostMoveAction", new DeterminePostMoveActionEngine(sequences["boardPress"]));

            engines.Add("forcedRecoveryCheck", new ForcedRecoveryCheckEngine(sequences["boardPress"]));
            engines.Add("forcedRecoveryAbility", new ForcedRecoveryAbilityEngine(sequences["boardPress"]));

            engines.Add("forcedRearrangementCheck", new ForcedRearrangementCheckEngine(sequences["boardPress"]));
            engines.Add("forcedRearrangementAbility", new ForcedRearrangementAbilityEngine(sequences["boardPress"]));
            engines.Add("gotoForcedRearrangement", new GotoForcedRearrangementEngine(sequences["boardPress"]));

            engines.Add("betrayal", new BetrayalEngine(sequences["boardPress"]));

            engines.Add("determineClickType", new DetermineClickTypeEngine(sequences["boardPress"]));
            engines.Add("towerModal", new TowerModalEngine());
            engines.Add("cancelModal", new CancelModalEngine(sequences["cancelModal"]));
            engines.Add("towerModalAnswer", new TowerModalAnswerEngine(sequences["towerModalAnswer"]));
            engines.Add("dropModal", new DropModalEngine());
            engines.Add("dropModalAnswer", new DropModalAnswerEngine(sequences["dropModalAnswer"]));
            engines.Add("confirmModal", new ConfirmModalEngine());
            engines.Add("confirmModalAnswer", new ConfirmModalAnswerEngine(sequences["confirmModalAnswer"]));

            engines.Add("captureStackModal", new CaptureStackModalEngine());
            engines.Add("captureStackModalAnswer", new CaptureStackModalAnswerEngine(sequences["captureStackModalAnswer"]));

            engines.Add("designateImmobileCapture", new DesignateImmobileCaptureEngine());
            engines.Add("immobileCapture", new ImmobileCaptureEngine());
            engines.Add("gotoTurnEnd", new GotoTurnEndEngine(sequences["towerModalAnswer"]));
        }

        public void AddEngines()
        {
            enginesRoot.AddEngine(engines["piecePress"]);
            enginesRoot.AddEngine(engines["tilePress"]);
            enginesRoot.AddEngine(engines["unPress"]);
            enginesRoot.AddEngine(engines["boardPress"]);
            enginesRoot.AddEngine(engines["deHighlightTeamPieces"]);
            enginesRoot.AddEngine(engines["pieceHighlight"]);
            enginesRoot.AddEngine(engines["tileHighlight"]);

            enginesRoot.AddEngine(engines["determineMoveType"]);

            enginesRoot.AddEngine(engines["unHighlight"]);
            enginesRoot.AddEngine(engines["movePiece"]);
            enginesRoot.AddEngine(engines["movePieceCleanup"]);
            enginesRoot.AddEngine(engines["turnEnd"]);

            enginesRoot.AddEngine(engines["commanderCheck"]);
            enginesRoot.AddEngine(engines["highlightAllDestinationTiles"]);

            enginesRoot.AddEngine(engines["mobileCapturePiece"]);
            enginesRoot.AddEngine(engines["addPieceToHand"]);
            enginesRoot.AddEngine(engines["gotoMovePiece"]);

            enginesRoot.AddEngine(engines["handPiecePress"]);
            enginesRoot.AddEngine(engines["handPieceHighlight"]);

            enginesRoot.AddEngine(engines["dropCheckStatus"]);
            enginesRoot.AddEngine(engines["preDropAbilities"]);
            enginesRoot.AddEngine(engines["drop"]);

            enginesRoot.AddEngine(engines["determinePostMoveAction"]);

            enginesRoot.AddEngine(engines["forcedRecoveryCheck"]);
            enginesRoot.AddEngine(engines["forcedRecoveryAbility"]);

            enginesRoot.AddEngine(engines["forcedRearrangementCheck"]);
            enginesRoot.AddEngine(engines["forcedRearrangementAbility"]);
            enginesRoot.AddEngine(engines["gotoForcedRearrangement"]);

            enginesRoot.AddEngine(engines["betrayal"]);

            enginesRoot.AddEngine(engines["determineClickType"]);
            enginesRoot.AddEngine(engines["towerModal"]);
            enginesRoot.AddEngine(engines["cancelModal"]);
            enginesRoot.AddEngine(engines["towerModalAnswer"]);
            enginesRoot.AddEngine(engines["dropModal"]);
            enginesRoot.AddEngine(engines["dropModalAnswer"]);
            enginesRoot.AddEngine(engines["confirmModal"]);
            enginesRoot.AddEngine(engines["confirmModalAnswer"]);

            enginesRoot.AddEngine(engines["captureStackModal"]);
            enginesRoot.AddEngine(engines["captureStackModalAnswer"]);

            enginesRoot.AddEngine(engines["designateImmobileCapture"]);
            enginesRoot.AddEngine(engines["immobileCapture"]);
            enginesRoot.AddEngine(engines["gotoTurnEnd"]);
        }
    }
}
