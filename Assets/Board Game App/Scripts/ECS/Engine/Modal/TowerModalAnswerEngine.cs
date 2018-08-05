using Data.Enum;
using Data.Step;
using ECS.EntityView.Modal;
using ECS.EntityView.Piece;
using Service.Board;
using Service.Piece;
using Svelto.ECS;

namespace ECS.Engine.Modal
{
    class TowerModalAnswerEngine : SingleEntityEngine<ModalEV>, IQueryingEntitiesEngine
    {
        private readonly ISequencer towerModalConfirmSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public TowerModalAnswerEngine(ISequencer towerModalConfirmSequence)
        {
            this.towerModalConfirmSequence = towerModalConfirmSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref ModalEV entityView)
        {
            entityView.answer.Answer.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref ModalEV entityView)
        {
            entityView.answer.Answer.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, int pieceReferenceId)
        {
            PieceEV piece = FindAssociatedPiece(pieceReferenceId);

            var pressState = new PressStepState
            {
                pieceEntityId = pieceReferenceId,
                piecePressState = piece.highlight.IsHighlighted ? PiecePressState.UNCLICKED : PiecePressState.CLICKED,
                affectedTiles = DestinationTileService.CalcDestinationTileLocations(piece, entitiesDB)
            };

            towerModalConfirmSequence.Next(this, ref pressState);
        }

        private PieceEV FindAssociatedPiece(int pieceId)
        {
            return PieceService.FindPieceEV(pieceId, entitiesDB);
        }
    }
}
