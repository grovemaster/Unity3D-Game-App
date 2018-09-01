using Data.Enums.Piece.Side;
using Data.Step.Drop;
using ECS.EntityView.Modal;
using Service.Board.Tile;
using Service.Hand;
using Service.Modal;
using Svelto.ECS;

namespace ECS.Engine.Modal.Drop
{
    class DropModalAnswerEngine : SingleEntityEngine<ModalEV>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();
        private ModalService modalService = new ModalService();
        private TileService tileService = new TileService();

        private readonly ISequencer dropModalAnswerSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public DropModalAnswerEngine(ISequencer dropModalAnswerSequence)
        {
            this.dropModalAnswerSequence = dropModalAnswerSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref ModalEV entityView)
        {
            entityView.DropFrontBackModal.Answer.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref ModalEV entityView)
        {
            entityView.DropFrontBackModal.Answer.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, PieceSide answer)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            modal.Visibility.IsVisible.value = false; // Forcibly close confirm modal
            NextAction(modal, answer);
        }

        private void NextAction(ModalEV modal, PieceSide answer)
        {
            var dropToken = new DropStepState
            {
                HandPiece = handService.FindHandPiece(modal.DropFrontBackModal.HandPieceReferenceId, entitiesDB),
                DestinationTile = tileService.FindTileEV(modal.DropFrontBackModal.TileReferenceId, entitiesDB),
                Side = modal.DropFrontBackModal.Answer.value
            };

            dropModalAnswerSequence.Next(this, ref dropToken);
        }
    }
}
