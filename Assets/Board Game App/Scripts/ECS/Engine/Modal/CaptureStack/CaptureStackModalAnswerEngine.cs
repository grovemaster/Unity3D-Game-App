using Data.Enum;
using Data.Enum.Modal;
using Data.Enum.Move;
using Data.Step;
using Data.Step.Piece.Capture;
using Data.Step.Piece.Move;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Modal;
using ECS.EntityView.Piece;
using Service.Board;
using Service.Board.Tile;
using Service.Modal;
using Service.Piece;
using Svelto.ECS;
using System;

namespace ECS.Engine.Modal.CaptureStack
{
    class CaptureStackModalAnswerEngine : SingleEntityEngine<ModalEV>, IQueryingEntitiesEngine
    {
        private readonly ISequencer captureStackModalAnswerSequence;
        private ModalService modalService = new ModalService();

        public IEntitiesDB entitiesDB { private get; set; }

        public CaptureStackModalAnswerEngine(ISequencer captureStackModalAnswerSequence)
        {
            this.captureStackModalAnswerSequence = captureStackModalAnswerSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref ModalEV entityView)
        {
            entityView.CaptureOrStack.Answer.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref ModalEV entityView)
        {
            entityView.CaptureOrStack.Answer.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, ModalQuestionAnswer answer)
        {
            switch (answer)
            {
                case ModalQuestionAnswer.CAPTURE:
                    NextActionCapture();
                    break;
                case ModalQuestionAnswer.STACK:
                    NextActionStack();
                    break;
                default:
                    throw new InvalidOperationException("Unsupported ModalQuestionAnswer value");
            }
        }

        private void NextActionCapture()
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            TileEV destinationTile = FindDestinationTile(modal);
            PieceEV topPieceAtDestinationTile = PieceService.FindTopPieceByLocation(
                destinationTile.Location.Location, entitiesDB).Value;
            PieceEV pieceToMove = FindPieceToMove(destinationTile);

            var captureToken = new CapturePieceStepState
            {
                pieceToCapture = topPieceAtDestinationTile,
                pieceToMove = pieceToMove,
                destinationTile = destinationTile
            };

            captureStackModalAnswerSequence.Next(this, ref captureToken, (int)MoveState.MOBILE_CAPTURE);
        }

        private void NextActionStack()
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            TileEV destinationTile = FindDestinationTile(modal);
            PieceEV pieceToMove = FindPieceToMove(destinationTile);

            var movePieceStepState = new MovePieceStepState
            {
                destinationTile = destinationTile,
                pieceToMove = pieceToMove
            };

            captureStackModalAnswerSequence.Next(this, ref movePieceStepState, (int)MoveState.MOVE_PIECE);
        }

        private TileEV FindDestinationTile(ModalEV modal)
        {
            return TileService.FindTileEV(modal.CaptureOrStack.TileReferenceId, entitiesDB);
        }

        private PieceEV FindPieceToMove(TileEV destinationTile)
        {
            return PieceService.FindPieceEVById(
                destinationTile.Tile.PieceRefEntityId.Value, entitiesDB).Value;
        }
    }
}
