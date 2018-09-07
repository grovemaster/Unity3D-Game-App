using Data.Enums.Piece;
using Data.Enums.Piece.Side;
using Data.Enums.Player;
using ECS.Context.EngineStep;
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

            new SetupEngines(enginesRoot, entityFactory).Setup();
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
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.COMMANDER, PieceType.COMMANDER, PieceSide.FRONT, 4, 1);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.COMMANDER, PieceType.COMMANDER, PieceSide.FRONT, 6, 5);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 0, 0);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 0, 1);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 0, 2);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 0, 3);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 1, 7);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 2, 1);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.SILVER, PieceSide.FRONT, 2, 7);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.GOLD, PieceSide.FRONT, 4, 2);

            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 0, 5);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 0, 6);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 0, 7);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 0, 8);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 1, 8);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceSide.FRONT, 3, 7);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.SILVER, PieceSide.FRONT, 4, 6);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.GOLD, PieceSide.FRONT, 4, 7);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.BOW, PieceType.ARROW, PieceSide.FRONT, 2, 0);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.BOW, PieceType.ARROW, PieceSide.FRONT, 2, 8);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.SPY, PieceType.CLANDESTINITE, PieceSide.FRONT, 3, 0);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.SPY, PieceType.CLANDESTINITE, PieceSide.BACK, 3, 4);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.CATAPULT, PieceType.LANCE, PieceSide.BACK, 5, 1);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.CATAPULT, PieceType.LANCE, PieceSide.BACK, 5, 8);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.FORTRESS, PieceType.LANCE, PieceSide.FRONT, 1, 0);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.FORTRESS, PieceType.LANCE, PieceSide.FRONT, 3, 8);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceSide.BACK, 7, 8);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.CATAPULT, PieceType.LANCE, PieceSide.BACK, 6, 8);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.HIDDEN_DRAGON, PieceType.DRAGON_KING, PieceSide.FRONT, 6, 1);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.HIDDEN_DRAGON, PieceType.DRAGON_KING, PieceSide.FRONT, 7, 0);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.HIDDEN_DRAGON, PieceType.DRAGON_KING, PieceSide.FRONT, 8, 0);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.HIDDEN_DRAGON, PieceType.DRAGON_KING, PieceSide.FRONT, 7, 7);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PRODIGY, PieceType.PHOENIX, PieceSide.FRONT, 7, 1);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PRODIGY, PieceType.PHOENIX, PieceSide.FRONT, 7, 6);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.SAMURAI, PieceType.PIKE, PieceSide.FRONT, 2, 2);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.SAMURAI, PieceType.PIKE, PieceSide.FRONT, 2, 6);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.CAPTAIN, PieceType.PISTOL, PieceSide.FRONT, 4, 3);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.CAPTAIN, PieceType.PISTOL, PieceSide.FRONT, 4, 5);
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
            CreateTeamHandPieces(handPieceCreateService, PlayerColor.BLACK);
            CreateTeamHandPieces(handPieceCreateService, PlayerColor.WHITE);
        }

        private void BuildModalEntity()
        {
            GameObject modalPanel = GameObject.Find("ModalPanel");

            entityFactory.BuildEntity<ModalED>(modalPanel.GetInstanceID(), modalPanel.GetComponents<IImplementor>());
        }

        private void CreateTeamHandPieces(HandPieceCreateService handPieceCreateService, PlayerColor playerColor)
        {
            int index = 0;
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.PAWN, PieceType.BRONZE, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.PAWN, PieceType.SILVER, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.PAWN, PieceType.GOLD, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.SPY, PieceType.CLANDESTINITE, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.CATAPULT, PieceType.LANCE, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.FORTRESS, PieceType.LANCE, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.HIDDEN_DRAGON, PieceType.DRAGON_KING, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.PRODIGY, PieceType.PHOENIX, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.BOW, PieceType.ARROW, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.SAMURAI, PieceType.PIKE, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.CAPTAIN, PieceType.PISTOL, index++);
            handPieceCreateService.CreateHandPiece(playerColor, PieceType.COMMANDER, PieceType.COMMANDER, index++);
        }
    }
}
