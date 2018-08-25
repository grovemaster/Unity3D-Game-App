using Data.Step.Board;
using ECS.EntityView.Piece;
using Svelto.ECS;

namespace ECS.Engine.Piece
{
    class PiecePressEngine : SingleEntityEngine<PieceEV>, IQueryingEntitiesEngine
    {
        private readonly ISequencer boardPressSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public PiecePressEngine(ISequencer boardPressSequence)
        {
            this.boardPressSequence = boardPressSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref PieceEV entityView)
        {
            entityView.Highlight.IsPressed.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref PieceEV entityView)
        {
            entityView.Highlight.IsPressed.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, bool isPressed)
        {
            if (!isPressed)
            {
                return;
            }

            var pressState = new BoardPressStepState
            {
                PieceEntityId = entityId,
                TileEntityId = null
            };
            
            boardPressSequence.Next(this, ref pressState);

        }
    }
}
