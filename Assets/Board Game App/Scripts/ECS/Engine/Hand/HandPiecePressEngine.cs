using Data.Step.Hand;
using ECS.EntityView.Hand;
using ECS.EntityView.Turn;
using Service.Hand;
using Service.Turn;
using Svelto.ECS;

namespace ECS.Engine.Hand
{
    class HandPiecePressEngine : SingleEntityEngine<HandPieceEV>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();
        private TurnService turnService = new TurnService();

        private readonly ISequencer boardPressSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public HandPiecePressEngine(ISequencer boardPressSequence)
        {
            this.boardPressSequence = boardPressSequence;
        }

        public void Ready()
        { }

        protected override void Add(ref HandPieceEV entityView)
        {
            entityView.Highlight.IsPressed.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref HandPieceEV entityView)
        {
            entityView.Highlight.IsPressed.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, bool isPressed)
        {
            if (!isPressed)
            {
                return;
            }

            // if not turn team or num piece count == 0, stop
            HandPieceEV handPiece = handService.FindHandPiece(entityId, entitiesDB);
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);

            if (handPiece.PlayerOwner.PlayerColor != currentTurn.TurnPlayer.PlayerColor
                || handPiece.HandPiece.NumPieces.value <= 0)
            {
                handPiece.Highlight.IsPressed.value = false; // Will trigger a HandPiecePressEngine, but IsPressed check will stop it
                return;
            }

            var pressState = new HandPiecePressStepState
            {
                HandPieceEntityId = entityId
            };

            boardPressSequence.Next(this, ref pressState);
        }
    }
}
