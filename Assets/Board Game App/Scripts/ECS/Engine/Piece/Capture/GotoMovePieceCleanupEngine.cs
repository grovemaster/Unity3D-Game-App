using Data.Step.Piece.Capture;
using Data.Step.Piece.Move;
using Service.Board.Tile;
using Svelto.ECS;

namespace ECS.Engine.Piece.Capture
{
    class GotoMovePieceCleanupEngine : IStep<ImmobileCapturePieceStepState>, IQueryingEntitiesEngine
    {
        private Sequencer towerModalAnswerSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public GotoMovePieceCleanupEngine(Sequencer towerModalAnswerSequence)
        {
            this.towerModalAnswerSequence = towerModalAnswerSequence;
        }

        public void Ready()
        { }

        public void Step(ref ImmobileCapturePieceStepState token, int condition)
        {
            // Dummy data, ignore.  Later improvment is to rework engines to not require those parameters in the first place
            var movePieceInfo = new MovePieceStepState
            {
                pieceToMove = token.pieceToCapture,
                destinationTile = TileService.FindAllTileEVs(entitiesDB)[0]
            };

            towerModalAnswerSequence.Next(this, ref movePieceInfo);
        }
    }
}
