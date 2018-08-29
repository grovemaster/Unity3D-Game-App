using Data.Step.Piece.Ability.ForcedRearrangement;
using Data.Step.Piece.Move;
using Svelto.ECS;

namespace Engine.Piece.Ability.ForcedRearrangement.Goto
{
    class GotoForcedRearrangementEngine : IStep<ForcedRecoveryStepState>, IQueryingEntitiesEngine
    {
        private Sequencer gotoForcedRearrangement;

        public IEntitiesDB entitiesDB { private get; set; }

        public GotoForcedRearrangementEngine(Sequencer gotoForcedRearrangement)
        {
            this.gotoForcedRearrangement = gotoForcedRearrangement;
        }

        public void Ready()
        { }

        public void Step(ref ForcedRecoveryStepState token, int condition)
        {
            var forcedRearrangementToken = new ForcedRearrangementStepState
            {
                PieceToRearrange = token.PieceCaptured
            };

            gotoForcedRearrangement.Next(this, ref forcedRearrangementToken);
        }
    }
}
