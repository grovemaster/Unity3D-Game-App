using Data.Enum;
using Data.Enum.Player;
using Data.Step;
using Data.Step.Board;
using Data.Step.Piece.Move;
using ECS.Engine.Board;
using ECS.Engine.Board.Tile;
using ECS.Engine.Board.Tile.Highlight;
using ECS.Engine.Piece;
using ECS.Engine.Piece.Move;
using ECS.Engine.Turn;
using ECS.EntityDescriptor.Turn;
using ECS.Implementor;
using ECS.Implementor.Turn;
using PrefabUtil;
using Service.Board.Context;
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

            var piecePressEngine = new PiecePressEngine(boardPressSequence);
            var tilePressEngine = new TilePressEngine(boardPressSequence);
            var unPressEngine = new UnPressEngine();
            var boardPressEngine = new BoardPressEngine(boardPressSequence);
            var deHighlightTeamPiecesEngine = new DeHighlightTeamPiecesEngine();
            var pieceHighlightEngine = new PieceHighlightEngine();
            var tileHighlightEngine = new TileHighlightEngine();

            var unHighlightEngine = new UnHighlightEngine();
            var movePieceEngine = new MovePieceEngine();
            var movePieceCleanupEngine = new MovePieceCleanupEngine();
            var turnEndEngine = new TurnEndEngine(boardPressSequence);

            var highlightAllDestinationTilesEngine = new HighlightAllDestinationTilesEngine();

            boardPressSequence.SetSequence(
                new Steps
                {
                    { // first step
                        piecePressEngine,
                        new To
                        {
                            new IStep<BoardPressStepState>[] { unPressEngine, boardPressEngine }
                        }
                    },
                    { // also first step
                        tilePressEngine,
                        new To
                        {
                            new IStep<BoardPressStepState>[] { unPressEngine, boardPressEngine }
                        }
                    },
                    {   // Clicking on board results in...
                        boardPressEngine,
                        new To
                        {   // Highlight piece and tile(s)
                            { (int)BoardPress.CLICK_HIGHLIGHT,
                                new IStep<PressStepState>[]
                                { deHighlightTeamPiecesEngine, pieceHighlightEngine, tileHighlightEngine } },
                            // Move piece
                            { (int)BoardPress.MOVE_PIECE,
                                new IStep<MovePieceStepState>[]
                                { unHighlightEngine, movePieceEngine, movePieceCleanupEngine, turnEndEngine } }
                        }
                    },
                    {   // Turn Start
                        turnEndEngine,
                        new To
                        {
                            highlightAllDestinationTilesEngine
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

            enginesRoot.AddEngine(unHighlightEngine);
            enginesRoot.AddEngine(movePieceEngine);
            enginesRoot.AddEngine(movePieceCleanupEngine);
            enginesRoot.AddEngine(turnEndEngine);

            enginesRoot.AddEngine(highlightAllDestinationTilesEngine);
        }

        private void SetupEntities() {
            BuildPieceEntities();
            BuildTileEntities();
            BuildTurnEntity();
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
            pieceCreateService.CreatePiece(PlayerColor.BLACK, 2, 1);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, 4, 2);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 1, 8);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 3, 7);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, 4, 6);
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
    }
}
