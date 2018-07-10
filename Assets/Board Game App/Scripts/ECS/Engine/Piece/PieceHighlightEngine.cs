using Data.Enum;
using Data.Step;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;

namespace ECS.Engine.Piece
{
    public class PieceHighlightEngine : IStep<PressState>, IQueryingEntitiesEngine
    {
        private readonly ISequencer highlightSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public PieceHighlightEngine(ISequencer highlightSequence)
        {
            this.highlightSequence = highlightSequence;
        }

        public void Ready()
        { }

        public void Step(ref PressState token, int condition)
        {
            PieceEV piece = PieceService.FindPieceEV(token.pieceEntityId, entitiesDB);

            piece.highlight.CurrentColor.value = token.piecePressState.Equals(PiecePressState.CLICKED)
                ? HighlightState.CLICKED : HighlightState.DEFAULT;
        }
    }
}
