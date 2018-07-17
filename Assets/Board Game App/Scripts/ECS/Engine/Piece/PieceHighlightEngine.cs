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
            HighlightState colorToChange = HighlightService.CalcHighlightState(piece.playerOwner.PlayerColor);

            entitiesDB.ExecuteOnEntity(
                piece.ID,
                (ref PieceEV pieceToChange) =>
                {
                    pieceToChange.highlight.IsHighlighted = isClicked;

                    if (isClicked)
                    {
                        pieceToChange.highlight.CurrentColorStates.Add(colorToChange);
                    }
                    else
                    {
                        pieceToChange.highlight.CurrentColorStates.Remove(colorToChange);
                    }
                });

            piece.changeColorComponent.PlayChangeColor = true;
        }
    }
}
