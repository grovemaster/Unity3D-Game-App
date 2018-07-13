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
            entityView.highlight.IsHighlighted.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref TileEV entityView)
        {
            entityView.highlight.IsHighlighted.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, bool isHighlighted)
        {
            var pressState = new BoardPressState
            {
                pieceEntityId = null,
                tileEntityId = entityId
            };

            boardPressSequence.Next(this, ref pressState);
        }
    }
}
