using Data.Step.Board;
using ECS.EntityView.Board.Tile;
using Svelto.ECS;

namespace ECS.Engine.Board.Tile
{
    class TilePressEngine : SingleEntityEngine<TileEV>, IQueryingEntitiesEngine
    {
        private readonly ISequencer boardPressSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public TilePressEngine(ISequencer boardPressSequence)
        {
            this.boardPressSequence = boardPressSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref TileEV entityView)
        {
            entityView.Highlight.IsPressed.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref TileEV entityView)
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
                pieceEntityId = null,
                tileEntityId = entityId
            };

            boardPressSequence.Next(this, ref pressState);
        }
    }
}
