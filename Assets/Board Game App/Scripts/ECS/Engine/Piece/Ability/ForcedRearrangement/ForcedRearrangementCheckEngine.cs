using Data.Enums.AB;
using Data.Enums.Piece;
using Data.Enums.Piece.PostMove;
using Data.Piece;
using Data.Step.Piece.Ability.ForcedRearrangement;
using Service.Piece.Factory;
using Svelto.ECS;

namespace ECS.Engine.Piece.Ability.ForcedRearrangement
{
    class ForcedRearrangementCheckEngine : IStep<ForcedRearrangementStepState>, IQueryingEntitiesEngine
    {
        private PieceFactory pieceFactory = new PieceFactory();

        private readonly ISequencer forcedRearrangementSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public ForcedRearrangementCheckEngine(ISequencer forcedRearrangementSequence)
        {
            this.forcedRearrangementSequence = forcedRearrangementSequence;
        }

        public void Ready()
        { }

        public void Step(ref ForcedRearrangementStepState token, int condition)
        {
            // Want current side of piece, not any side for determining Forced Rearrangement
            bool initiateForcedRearrangement = token.PieceToRearrange.HasValue
                && HasForcedRearrangementAbility(token.PieceToRearrange.Value.Piece.PieceType);
            NextAction(ref token, initiateForcedRearrangement);
        }

        private bool HasForcedRearrangementAbility(PieceType pieceType)
        {
            IPieceData pieceData = pieceFactory.CreateIPieceData(pieceType);

            return pieceData.Abilities.PostMove.Contains(PostMoveAbility.FORCED_REARRANGEMENT);
        }

        private void NextAction(ref ForcedRearrangementStepState token, bool initiateForcedRearrangement)
        {
            forcedRearrangementSequence.Next(this, ref token, initiateForcedRearrangement ? (int)StepAB.A : (int)StepAB.B);
        }
    }
}
