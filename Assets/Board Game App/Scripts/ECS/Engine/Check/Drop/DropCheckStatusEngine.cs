using Data.Constants.Board;
using Data.Step.Drop;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Check;
using Service.Drop;
using Service.Hand;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;

namespace Engine.Check.Drop
{
    class DropCheckStatusEngine : IStep<DropPrepStepState>, IQueryingEntitiesEngine
    {
        private CheckService checkService = new CheckService();
        private DropService dropService = new DropService();
        private HandService handService = new HandService();
        private PieceFindService pieceFindService = new PieceFindService();
        private TurnService turnService = new TurnService();

        private readonly ISequencer dropSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public DropCheckStatusEngine(ISequencer dropSequence)
        {
            this.dropSequence = dropSequence;
        }

        public void Ready()
        { }

        public void Step(ref DropPrepStepState token, int condition)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            PieceEV pieceToDrop = pieceFindService.FindFirstPieceByLocationAndType(
                BoardConst.HAND_LOCATION, token.HandPiece.HandPiece.PieceType, entitiesDB);

            if (!currentTurn.Check.CommanderInCheck
                || checkService.DropReleasesCheck(pieceToDrop, token.DestinationTile.Location.Location, currentTurn, entitiesDB))
            {
                NextAction(ref token);
            }
        }

        private void NextAction(ref DropPrepStepState token)
        {
            dropSequence.Next(this, ref token);
        }
    }
}
