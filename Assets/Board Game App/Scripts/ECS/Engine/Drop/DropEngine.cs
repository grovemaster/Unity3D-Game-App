using Data.Constants.Board;
using Data.Step.Drop;
using Data.Step.Turn;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using Service.Drop;
using Service.Hand;
using Service.Piece.Find;
using Svelto.ECS;

namespace ECS.Engine.Drop
{
    class DropEngine : IStep<DropStepState>, IQueryingEntitiesEngine
    {
        private DropService dropService = new DropService();
        private HandService handService = new HandService();
        private PieceFindService pieceFindService = new PieceFindService();

        private readonly ISequencer dropSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public DropEngine(ISequencer dropSequence)
        {
            this.dropSequence = dropSequence;
        }

        public void Ready()
        { }

        public void Step(ref DropStepState token, int condition)
        {
            PieceEV pieceToDrop = pieceFindService.FindFirstPieceByLocationAndType(
                BoardConst.HAND_LOCATION, token.HandPiece.HandPiece.PieceType, token.HandPiece.HandPiece.Back, entitiesDB);

            dropService.DropPiece(
                ref pieceToDrop,
                ref token.DestinationTile,
                token.Side,
                token.HandPiece.PlayerOwner.PlayerColor,
                entitiesDB);
            UpdateHandPiece(ref token.HandPiece);
            GotoTurnEndStep();
        }

        private void UpdateHandPiece(ref HandPieceEV handPiece)
        {
            handService.DecrementHandPiece(ref handPiece);
        }

        private void GotoTurnEndStep()
        {
            var turnEndToken = new TurnEndStepState();
            dropSequence.Next(this, ref turnEndToken);
        }
    }
}
