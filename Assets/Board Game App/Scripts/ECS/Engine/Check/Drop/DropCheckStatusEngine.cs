using Data.Constants.Board;
using Data.Enums.AB;
using Data.Enums.Drop;
using Data.Enums.Piece.Side;
using Data.Step.Drop;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Checkmate;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;

namespace Engine.Check.Drop
{
    class DropCheckStatusEngine : IStep<DropPrepStepState>, IStep<DropStepState>, IQueryingEntitiesEngine
    {
        private CheckmateService checkmateService = new CheckmateService();
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
            // TODO CheckDropToken needs to return a state that allows a single-sided drop token
            PieceSideState resultState = CheckDropToken(ref token.HandPiece, ref token.DestinationTile, null);

            if (resultState == PieceSideState.BOTH)
            {
                NextAction(ref token);
            }
            else if (resultState == PieceSideState.FRONT || resultState == PieceSideState.BACK)
            {
                var dropToken = new DropStepState
                {
                    DestinationTile = token.DestinationTile,
                    HandPiece = token.HandPiece,
                    Side = resultState == PieceSideState.FRONT ? PieceSide.FRONT : PieceSide.BACK
                };

                NextAction(ref dropToken);
            }
        }

        public void Step(ref DropStepState token, int condition)
        {
            if (CheckDropToken(ref token.HandPiece, ref token.DestinationTile, token.Side) != PieceSideState.NONE)
            {
                NextAction(ref token);
            }
        }

        private PieceSideState CheckDropToken(ref HandPieceEV handPiece, ref TileEV destinationTile, PieceSide? sideToCheck)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);

            if (currentTurn.InitialArrangement.IsInitialArrangementInEffect)
            {
                return PieceSideState.FRONT;
            }

            PieceEV pieceToDrop = pieceFindService.FindFirstPieceByLocationAndType(
                BoardConst.HAND_LOCATION, handPiece.HandPiece.PieceType, handPiece.HandPiece.Back, entitiesDB);

            bool singleSideValid = sideToCheck.HasValue && checkmateService.DropReleasesCheck(
                pieceToDrop,
                destinationTile.Location.Location,
                currentTurn.TurnPlayer.PlayerColor,
                sideToCheck.Value,
                handPiece,
                entitiesDB);

            bool frontSideValid = !sideToCheck.HasValue
                && checkmateService.DropReleasesCheck(
                    pieceToDrop,
                    destinationTile.Location.Location,
                    currentTurn.TurnPlayer.PlayerColor,
                    PieceSide.FRONT,
                    handPiece,
                    entitiesDB);
            bool backSideValid = !sideToCheck.HasValue
                && checkmateService.DropReleasesCheck(
                    pieceToDrop,
                    destinationTile.Location.Location,
                    currentTurn.TurnPlayer.PlayerColor,
                    PieceSide.BACK,
                    handPiece,
                    entitiesDB);

            if (singleSideValid)
            {
                return sideToCheck.Value == PieceSide.FRONT ? PieceSideState.FRONT : PieceSideState.BACK;
            }
            else if (frontSideValid && backSideValid)
            {
                return PieceSideState.BOTH;
            }
            else if (frontSideValid)
            {
                return PieceSideState.FRONT;
            }
            else if (backSideValid)
            {
                return PieceSideState.BACK;
            }
            else
            {
                return PieceSideState.NONE;
            }
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
