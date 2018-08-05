using System;
using Data.Enum;
using Data.Enum.Click;
using Data.Enum.Move;
using Data.Enum.Player;
using Data.Step;
using Data.Step.Board;
using Data.Step.Drop;
using Data.Step.Hand;
using Data.Step.Piece.Capture;
using Data.Step.Piece.Click;
using Data.Step.Piece.Move;
using ECS.Engine.Board;
using ECS.Engine.Board.Tile;
using ECS.Engine.Board.Tile.Highlight;
using ECS.Engine.Drop;
using ECS.Engine.Hand;
using ECS.Engine.Hand.Highlight;
using ECS.Engine.Modal;
using ECS.Engine.Modal.CaptureStack;
using ECS.Engine.Move;
using ECS.Engine.Piece;
using ECS.Engine.Piece.Capture;
using ECS.Engine.Piece.Click;
using ECS.Engine.Piece.Move;
using ECS.Engine.Turn;
using ECS.EntityDescriptor.Modal;
using ECS.EntityDescriptor.Turn;
using ECS.Implementor;
using ECS.Implementor.Turn;
using PrefabUtil;
using Service.Board.Context;
using Service.Hand.Context;
using Service.Piece.Context;
using Svelto.Context;
using Svelto.ECS;
using Svelto.ECS.Schedulers.Unity;
using Svelto.Tasks;
using UnityEngine;

namespace ECS.Context
{
    /// <summary>
    ///At least One GameObject containing a UnityContext must be present in the scene.
    ///All the monobehaviours existing in gameobjects child of the UnityContext one, 
    ///can be later queried, usually to create entities from statically created
    ///gameobjects. 
    /// </summary>
    public class MainContext : UnityContext<Main> {}

    public class Main: IUnityCompositionRoot
    {
        private EnginesRoot enginesRoot;
        private IEntityFactory entityFactory;

        public Main()
        {
            InitAssets();
            SetupEngines();
            SetupEntities();
        }

        public void OnContextCreated(UnityContext contextHolder)
        {
            //BuildEntitiesFromScene(contextHolder);
            //throw new NotImplementedException();
        }

        public void OnContextDestroyed()
        {
            enginesRoot.Dispose();
            TaskRunner.StopAndCleanupAllDefaultSchedulers();
        }

        public void OnContextInitialized() {}

        private void InitAssets()
        {
            //Do not copy this. initially I thought it was a good idea to use
            //Json serialization to replace resources, but it's less convenient
            //than I thought
            //...is what the tutorial example project says.  That's nice, but I don't know a more
            //elegant alternative, or even an alternative at all.  So I'll use something that
            //that works until further notice.
            GameObject.Find("PrefabsSerializer").GetComponent<PrefabsSerializer>().Init();
        }

        private void SetupEngines()
        {
            enginesRoot = new EnginesRoot(new UnitySumbmissionEntityViewScheduler());
            //Engines root can never be held by anything else than the context itself to avoid leaks
            //That's why the EntityFactory and EntityFunctions are generated.
            //The EntityFactory can be injected inside factories (or engine acting as factories)
            //to build new entities dynamically
            entityFactory = enginesRoot.GenerateEntityFactory();

            //the ISequencer is one of the 2 official ways available in Svelto.ECS 
            //to communicate. They are mainly used for two specific cases:
            //1) specify a strict execution order between engines (engine logic
            //is executed horizontally instead than vertically, I will talk about this
            //in my articles). 2) filter a data token passed as parameter through
            //engines. The ISequencer is also not the common way to communicate
            //between engines
            //...Then what is the common way to communicate between engines?  Querying entities?
            Sequencer boardPressSequence = new Sequencer();
            Sequencer handPiecePressSequence = new Sequencer();
            Sequencer cancelModalSequence = new Sequencer();
            Sequencer towerModalAnswerSequence = new Sequencer();
            Sequencer captureStackModalAnswerSequence = new Sequencer();

            var piecePressEngine = new PiecePressEngine(boardPressSequence);
            var tilePressEngine = new TilePressEngine(boardPressSequence);
            var unPressEngine = new UnPressEngine();
            var boardPressEngine = new BoardPressEngine(boardPressSequence);
            var deHighlightTeamPiecesEngine = new DeHighlightTeamPiecesEngine();
            var pieceHighlightEngine = new PieceHighlightEngine();
            var tileHighlightEngine = new TileHighlightEngine();

            var determineMoveTypeEngine = new DetermineMoveTypeEngine(boardPressSequence);

            var unHighlightEngine = new UnHighlightEngine();
            var movePieceEngine = new MovePieceEngine();
            var movePieceCleanupEngine = new MovePieceCleanupEngine();
            var turnEndEngine = new TurnEndEngine(boardPressSequence);

            var highlightAllDestinationTilesEngine = new HighlightAllDestinationTilesEngine();

            var mobileCapturePieceEngine = new MobileCapturePieceEngine();
            var addPieceToHandEngine = new AddPieceToHandEngine();
            var gotoMovePieceEngine = new GotoMovePieceEngine(boardPressSequence);

            var handPiecePressEngine = new HandPiecePressEngine(handPiecePressSequence);
            var handPieceHighlightEngine = new HandPieceHighlightEngine();

            var dropEngine = new DropEngine();

            var determineClickTypeEngine = new DetermineClickTypeEngine(boardPressSequence);
            var towerModalEngine = new TowerModalEngine();
            var cancelModalEngine = new CancelModalEngine(cancelModalSequence);
            var towerModalAnswerEngine = new TowerModalAnswerEngine(towerModalAnswerSequence);

            var captureStackModalEngine = new CaptureStackModalEngine();
            var captureStackModalAnswerEngine = new CaptureStackModalAnswerEngine(captureStackModalAnswerSequence);

            var pressStep = new IStep<BoardPressStepState>[] { unPressEngine, boardPressEngine };
            var clickStep = new IStep<PressStepState>[]
                                { deHighlightTeamPiecesEngine, pieceHighlightEngine, tileHighlightEngine };
            var movePieceStep = new IStep<MovePieceStepState>[]
                { unHighlightEngine, movePieceEngine, movePieceCleanupEngine, turnEndEngine };
            var capturePieceStep = new IStep<CapturePieceStepState>[]
                { mobileCapturePieceEngine, addPieceToHandEngine, gotoMovePieceEngine };
            var dropStep = new IStep<DropStepState>[]
                { dropEngine, unHighlightEngine, turnEndEngine };

            boardPressSequence.SetSequence(
                new Steps
                {
                    { // first step
                        piecePressEngine,
                        new To
                        {
                            pressStep
                        }
                    },
                    { // also first step
                        tilePressEngine,
                        new To
                        {
                            pressStep
                        }
                    },
                    {   // Clicking on board results in...
                        boardPressEngine,
                        new To
                        {   // Highlight piece and tile(s)
                            { (int)BoardPress.CLICK_HIGHLIGHT, new IStep<ClickPieceStepState>[] { determineClickTypeEngine } },
                            // Move piece or capture piece
                            { (int)BoardPress.MOVE_PIECE,
                                new IStep<MovePieceStepState>[] { determineMoveTypeEngine } },
                            // Drop piece
                            { (int)BoardPress.DROP, dropStep }
                        }
                    },
                    {
                        determineClickTypeEngine,
                        new To
                        {
                            { (int)ClickState.CLICK_HIGHLIGHT, clickStep },
                            { (int)ClickState.TOWER_MODAL, new IStep<ClickPieceStepState>[] { towerModalEngine } }
                        }
                    },
                    {   // Turn Start
                        turnEndEngine,
                        new To
                        {
                            highlightAllDestinationTilesEngine
                        }
                    },
                    {
                        determineMoveTypeEngine,
                        new To
                        {
                            { (int)MoveState.MOVE_PIECE, movePieceStep },
                            { (int)MoveState.MOBILE_CAPTURE, capturePieceStep },
                            { (int)MoveState.CAPTURE_STACK_MODAL, new IStep<CapturePieceStepState>[] { captureStackModalEngine } }
                        }
                    },
                    {
                        // After capturing opponent piece, still need to move turn piece
                        gotoMovePieceEngine,
                        new To
                        {
                            movePieceStep
                        }
                    }
                }
                );

            handPiecePressSequence.SetSequence(
                new Steps
                {
                    { // first step
                        handPiecePressEngine,
                        new To
                        {
                            new IStep<HandPiecePressStepState>[]
                            { deHighlightTeamPiecesEngine, handPieceHighlightEngine }
                        }
                    }
                }
                );

            cancelModalSequence.SetSequence(
                new Steps
                {
                    {
                        cancelModalEngine,
                        new To
                        {
                            deHighlightTeamPiecesEngine
                        }
                    }
                }
                );

            towerModalAnswerSequence.SetSequence(
                new Steps
                {
                    {
                        towerModalAnswerEngine,
                        new To
                        {
                            clickStep
                        }
                    }
                }
                );

            captureStackModalAnswerSequence.SetSequence(
                new Steps
                {
                    {
                        captureStackModalAnswerEngine,
                        new To
                        {
                            { (int)MoveState.MOVE_PIECE, movePieceStep },
                            { (int)MoveState.MOBILE_CAPTURE, capturePieceStep },
                        }
                    }
                }
                );

            enginesRoot.AddEngine(piecePressEngine);
            enginesRoot.AddEngine(tilePressEngine);
            enginesRoot.AddEngine(unPressEngine);
            enginesRoot.AddEngine(boardPressEngine);
            enginesRoot.AddEngine(deHighlightTeamPiecesEngine);
            enginesRoot.AddEngine(pieceHighlightEngine);
            enginesRoot.AddEngine(tileHighlightEngine);

            enginesRoot.AddEngine(determineMoveTypeEngine);

            enginesRoot.AddEngine(unHighlightEngine);
            enginesRoot.AddEngine(movePieceEngine);
            enginesRoot.AddEngine(movePieceCleanupEngine);
            enginesRoot.AddEngine(turnEndEngine);

            enginesRoot.AddEngine(highlightAllDestinationTilesEngine);

            enginesRoot.AddEngine(mobileCapturePieceEngine);
            enginesRoot.AddEngine(addPieceToHandEngine);
            enginesRoot.AddEngine(gotoMovePieceEngine);

            enginesRoot.AddEngine(handPiecePressEngine);
            enginesRoot.AddEngine(handPieceHighlightEngine);

            enginesRoot.AddEngine(dropEngine);

            enginesRoot.AddEngine(determineClickTypeEngine);
            enginesRoot.AddEngine(towerModalEngine);
            enginesRoot.AddEngine(cancelModalEngine);
            enginesRoot.AddEngine(towerModalAnswerEngine);

            enginesRoot.AddEngine(captureStackModalEngine);
            enginesRoot.AddEngine(captureStackModalAnswerEngine);
        }

        private void SetupEntities() {
            BuildPieceEntities();
            BuildTileEntities();
            BuildTurnEntity();
            BuildHandPieceEntities();
            BuildModalEntity();
        }

        private void BuildPieceEntities()
        {
            //var prefabsDictionary = new PrefabsDictionary();

            //var pawn = prefabsDictionary.Instantiate("Pawn");
            //pawn.transform.position = new Vector3(0, 0, 5);

            //Building entities explicitly should be always preferred
            //and MUST be used if an implementor doesn't need to be
            //a Monobehaviour. You should strive to create implementors
            //not as monobehaviours. Implementors as monobehaviours 
            //are meant only to function as bridge between Svelto.ECS
            //and Unity3D. Using implementor as monobehaviour
            //just to read serialized data from the editor, is also
            //a bad practice, use a Json file instead.
            //The Player Entity is made of EntityViewStruct+Implementors as monobehaviours and 
            //EntityStructs. The PlayerInputDataStruct doesn't need to be initialized (yay!!)
            //but the HealthEntityStruct does. Here I show the official method to do it
            //var initializer = entityFactory.BuildEntity<PieceED>(pawn.GetInstanceID(), pawn.GetComponents<IImplementor>());
            var pieceCreateService = new PieceCreateService(entityFactory);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, 0, 0);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, 0, 1);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, 0, 2);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, 0, 3);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, 2, 1);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, 4, 2);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 0, 5);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 0, 6);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 0, 7);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 0, 8);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 1, 8);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 3, 7);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 4, 6);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 4, 7);
        }

        private void BuildTileEntities()
        {
            var boardCreateService = new BoardCreateService(entityFactory);
            boardCreateService.CreateBoard();
        }

        private void BuildTurnEntity()
        {
            var prefabsDictionary = new PrefabsDictionary();
            var currentTurn = prefabsDictionary.Instantiate("Current Turn");
            var currentTurnImpl = currentTurn.GetComponent<TurnImpl>();
            entityFactory.BuildEntity<TurnED>(currentTurn.GetInstanceID(), currentTurn.GetComponents<IImplementor>());

            currentTurnImpl.PlayerColor = PlayerColor.BLACK;
        }

        private void BuildHandPieceEntities()
        {
            var handPieceCreateService = new HandPieceCreateService(entityFactory);
            handPieceCreateService.CreateHandPiece(PlayerColor.BLACK, PieceType.PAWN);
            handPieceCreateService.CreateHandPiece(PlayerColor.WHITE, PieceType.PAWN);
        }

        private void BuildModalEntity()
        {
            GameObject modalPanel = GameObject.Find("ModalPanel");

            entityFactory.BuildEntity<ModalED>(modalPanel.GetInstanceID(), modalPanel.GetComponents<IImplementor>());
        }
    }
}
