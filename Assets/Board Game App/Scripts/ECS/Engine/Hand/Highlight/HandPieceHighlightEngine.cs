using Data.Enum;
using Data.Step.Hand;
using ECS.EntityView.Hand;
using Service.Hand;
using Service.Highlight;
using Svelto.ECS;

namespace ECS.Engine.Hand.Highlight
{
    class HandPieceHighlightEngine : IStep<HandPiecePressStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();

        public HandPieceHighlightEngine()
        { }

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref HandPiecePressStepState token, int condition)
        {
            HandPieceEV handPiece = handService.FindHandPiece(token.handPieceEntityId, entitiesDB);
            bool isClicked = !handPiece.highlight.IsHighlighted;
            HighlightState colorToChange = HighlightService.CalcClickHighlightState(handPiece.playerOwner.PlayerColor);

            entitiesDB.ExecuteOnEntity(
                handPiece.ID,
                (ref HandPieceEV handPieceToChange) =>
                {
                    handPieceToChange.highlight.IsHighlighted = isClicked;

                    if (isClicked)
                    {
                        handPieceToChange.highlight.CurrentColorStates.Add(colorToChange);
                    }
                    else
                    {
                        handPieceToChange.highlight.CurrentColorStates.Clear();
                    }
                });

            handPiece.changeColor.PlayChangeColor = true;
        }
    }
}
