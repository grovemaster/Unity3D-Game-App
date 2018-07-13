﻿using Data.Step.Board;
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
            entityView.highlight.IsHighlighted.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref PieceEV entityView)
        {
            entityView.highlight.IsHighlighted.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, bool isHighlighted)
        {
            var pressState = new BoardPressState
            {
                pieceEntityId = entityId,
                tileEntityId = null
            };
            
            boardPressSequence.Next(this, ref pressState);

        }
    }
}
