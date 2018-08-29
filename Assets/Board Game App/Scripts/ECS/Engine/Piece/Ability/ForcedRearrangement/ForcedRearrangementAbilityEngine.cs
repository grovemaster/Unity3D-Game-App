using Data.Step.Piece.Ability.ForcedRearrangement;
using ECS.EntityView.Hand;
using ECS.EntityView.Turn;
using Service.Hand;
using Service.Turn;
using Svelto.ECS;

namespace ECS.Engine.Piece.Ability.ForcedRearrangement
{
    class ForcedRearrangementAbilityEngine : IStep<ForcedRearrangementStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();
        private TurnService turnService = new TurnService();

        private readonly ISequencer forcedRearrangementSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public ForcedRearrangementAbilityEngine(ISequencer forcedRearrangementSequence)
        {
            this.forcedRearrangementSequence = forcedRearrangementSequence;
        }

        public void Ready()
        { }

        public void Step(ref ForcedRearrangementStepState token, int condition)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            turnService.SetForcedRearrangementStatus(currentTurn, true, entitiesDB);

            HandPieceEV handPiece = handService.FindHandPiece(
                token.PieceToRearrange.Value.Piece.Front,
                token.PieceToRearrange.Value.Piece.Back,
                currentTurn.TurnPlayer.PlayerColor,
                entitiesDB);
            handService.HighlightHandPiece(ref handPiece, true, entitiesDB);
        }
    }
}
