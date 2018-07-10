using Data.Enum;
using Data.Step;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ECS.Engine.Piece
{
    class PiecePressEngine : SingleEntityEngine<PieceEV>, IQueryingEntitiesEngine
    {
        private readonly ISequencer peicePressSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public PiecePressEngine(ISequencer peicePressSequence)
        {
            this.peicePressSequence = peicePressSequence;
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
            var pressState = new PressState
            {
                pieceEntityId = entityId,
                piecePressState = isHighlighted ? PiecePressState.CLICKED : PiecePressState.UNCLICKED,
                affectedTiles = FindAffectedTiles(entityId)
            };

            peicePressSequence.Next(this, ref pressState);
        }

        private List<Vector3> FindAffectedTiles(int entityId)
        {
            PieceEV pieceEV = PieceService.FindPieceEV(entityId, entitiesDB);

            return PieceService.CreateIPieceData(pieceEV.piece.PieceType).Tiers()[0].Single()
                .Select(x => new Vector3(x.x, x.y, 0)).ToList(); // Change z-value from 1 to 0
        }
    }
}
