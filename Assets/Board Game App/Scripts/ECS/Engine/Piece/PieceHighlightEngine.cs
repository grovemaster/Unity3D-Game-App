using Data.Enum;
using Data.Step;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;

namespace ECS.Engine.Piece
{
    public class PieceHighlightEngine : IStep<PressStepState>, IQueryingEntitiesEngine
    {
        private readonly ISequencer highlightSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public PieceHighlightEngine(ISequencer highlightSequence)
        {
            this.highlightSequence = highlightSequence;
        }

        public void Ready()
        { }

        public void Step(ref PressStepState token, int condition)
        {
            PieceEV piece = PieceService.FindPieceEV(token.pieceEntityId, entitiesDB);
            bool isClicked = token.piecePressState.Equals(PiecePressState.CLICKED);

            entitiesDB.ExecuteOnEntity(
                piece.ID,
                (ref PieceEV pieceToChange) => { pieceToChange.highlight.IsHighlighted = isClicked ? true : false; });
            piece.highlight.CurrentColor.value = isClicked
                ? HighlightState.CLICKED : HighlightState.DEFAULT;
        }
    }
}
