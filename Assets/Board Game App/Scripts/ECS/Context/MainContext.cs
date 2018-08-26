using Data.Enum.Piece;
using Data.Enum.Player;
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
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.COMMANDER, PieceType.COMMANDER, PieceType.COMMANDER, 2, 4);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.COMMANDER, PieceType.COMMANDER, PieceType.COMMANDER, 6, 5);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 0, 0);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 0, 1);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 0, 2);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 0, 3);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 1, 7);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 2, 1);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 2, 7);
            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 4, 2);

            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 0, 5);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 0, 6);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 0, 7);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 0, 8);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 1, 8);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 3, 7);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 4, 6);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, PieceType.PAWN, 4, 7);

            pieceCreateService.CreatePiece(PlayerColor.BLACK, PieceType.BOW, PieceType.ARROW, PieceType.BOW, 2, 0);
            pieceCreateService.CreatePiece(PlayerColor.WHITE, PieceType.BOW, PieceType.ARROW, PieceType.BOW, 2, 8);
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
            int index = 0;
            handPieceCreateService.CreateHandPiece(PlayerColor.BLACK, PieceType.PAWN, PieceType.BRONZE, index++);
            handPieceCreateService.CreateHandPiece(PlayerColor.BLACK, PieceType.BOW, PieceType.ARROW, index++);
            handPieceCreateService.CreateHandPiece(PlayerColor.BLACK, PieceType.COMMANDER, PieceType.COMMANDER, index++);

            index = 0;
            handPieceCreateService.CreateHandPiece(PlayerColor.WHITE, PieceType.PAWN, PieceType.BRONZE, index++);
            handPieceCreateService.CreateHandPiece(PlayerColor.WHITE, PieceType.BOW, PieceType.ARROW, index++);
            handPieceCreateService.CreateHandPiece(PlayerColor.WHITE, PieceType.COMMANDER, PieceType.COMMANDER, index++);
        }

        private void BuildModalEntity()
        {
            GameObject modalPanel = GameObject.Find("ModalPanel");

            entityFactory.BuildEntity<ModalED>(modalPanel.GetInstanceID(), modalPanel.GetComponents<IImplementor>());
        }
    }
}
