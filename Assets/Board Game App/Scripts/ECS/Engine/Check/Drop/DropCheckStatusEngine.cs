using Data.Constants.Board;
using Data.Enums.AB;
using Data.Enums.Piece.Side;
using Data.Step.Drop;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;
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
    class DropCheckStatusEngine : IStep<DropPrepStepState>, IStep<DropStepState>, IQueryingEntitiesEngine
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
            if (CheckDropToken(ref token.HandPiece, ref token.DestinationTile, null))
            {
                NextAction(ref token);
            }
        }

        public void Step(ref DropStepState token, int condition)
        {
            if (CheckDropToken(ref token.HandPiece, ref token.DestinationTile, token.Side))
            {
                NextAction(ref token);
            }
        }

        private bool CheckDropToken(ref HandPieceEV handPiece, ref TileEV destinationTile, PieceSide? sideToCheck)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            PieceEV pieceToDrop = pieceFindService.FindFirstPieceByLocationAndType(
                BoardConst.HAND_LOCATION, handPiece.HandPiece.PieceType, handPiece.HandPiece.Back, entitiesDB);
            bool returnValue = !currentTurn.Check.CommanderInCheck;

            if (!returnValue) // Commander is in check
            {
                bool singleSideValid = sideToCheck.HasValue && checkService.DropReleasesCheck(
                    pieceToDrop,
                    destinationTile.Location.Location,
                    currentTurn,
                    sideToCheck.Value,
                    entitiesDB);

                bool eitherSideValid = !sideToCheck.HasValue && (
                    checkService.DropReleasesCheck(
                        pieceToDrop,
                        destinationTile.Location.Location,
                        currentTurn,
                        PieceSide.FRONT,
                        entitiesDB)
                    || checkService.DropReleasesCheck(
                        pieceToDrop,
                        destinationTile.Location.Location,
                        currentTurn,
                        PieceSide.BACK,
                        entitiesDB));

                returnValue = singleSideValid || eitherSideValid;
            }

            return returnValue;
        }

        private void NextAction(ref DropPrepStepState token)
        {
            dropSequence.Next(this, ref token, (int)StepAB.A);
        }

        private void NextAction(ref DropStepState token)
        {
            dropSequence.Next(this, ref token, (int)StepAB.B);
        }
    }
}
