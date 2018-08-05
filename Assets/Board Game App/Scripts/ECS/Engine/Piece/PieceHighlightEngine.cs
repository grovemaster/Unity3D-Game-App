using Data.Enum;
using Data.Step;
using ECS.EntityView.Piece;
using Service.Highlight;
using Service.Piece;
using Svelto.ECS;

namespace ECS.Engine.Piece
{
    public class PieceHighlightEngine : IStep<PressStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref PressStepState token, int condition)
        {
            PieceEV piece = PieceService.FindPieceEV(token.pieceEntityId, entitiesDB);
            bool isClicked = token.piecePressState == PiecePressState.CLICKED;
            HighlightState colorToChange = HighlightService.CalcClickHighlightState(piece.PlayerOwner.PlayerColor);

            entitiesDB.ExecuteOnEntity(
                piece.ID,
                (ref PieceEV pieceToChange) =>
                {
                    pieceToChange.Highlight.IsHighlighted = isClicked;

                    if (isClicked)
                    {
                        pieceToChange.Highlight.CurrentColorStates.Add(colorToChange);
                    }
                    else
                    {
                        pieceToChange.Highlight.CurrentColorStates.Remove(colorToChange);
                    }
                });

            piece.ChangeColorTrigger.PlayChangeColor = true;
        }
    }
}
