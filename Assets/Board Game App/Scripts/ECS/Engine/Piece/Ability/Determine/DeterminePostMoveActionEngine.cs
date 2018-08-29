using Data.Enum.AB;
using Data.Enum.Piece;
using Data.Enum.Piece.PostMove;
using Data.Piece;
using Data.Step.Piece.Ability;
using Data.Step.Piece.Ability.ForcedRearrangement;
using Data.Step.Piece.Move;
using Data.Step.Turn;
using Service.Piece.Factory;
using Svelto.ECS;

namespace ECS.Engine.Piece.Ability.Determine
{
    class DeterminePostMoveActionEngine : IStep<DeterminePostMoveStepState>, IQueryingEntitiesEngine
    {
        private PieceFactory pieceFactory = new PieceFactory();

        private readonly ISequencer determinePostMoveSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public DeterminePostMoveActionEngine(ISequencer determinePostMoveSequence)
        {
            this.determinePostMoveSequence = determinePostMoveSequence;
        }

        public void Ready()
        { }

        public void Step(ref DeterminePostMoveStepState token, int condition)
        {
            // Want current side of piece, not any side for determining Forced Rearrangement
            if (HasForcedRecoveryAbility(token.PieceMoved.Piece.PieceType))
            {
                NextActionForcedRecovery(ref token);
            }
            else if (token.PieceCaptured.HasValue && HasForcedRearrangementAbility(token.PieceCaptured.Value.Piece.PieceType))
            {
                NextActionForcedRearrangement(ref token);
            }
            else
            {
                NextActionTurnEnd();
            }
        }

        private bool HasForcedRecoveryAbility(PieceType pieceType)
        {
            IPieceData pieceData = pieceFactory.CreateIPieceData(pieceType);

            return pieceData.Abilities.PostMove.Contains(PostMoveAbility.FORCED_RECOVERY);
        }

        private bool HasForcedRearrangementAbility(PieceType pieceType)
        {
            IPieceData pieceData = pieceFactory.CreateIPieceData(pieceType);

            return pieceData.Abilities.PostMove.Contains(PostMoveAbility.FORCED_REARRANGEMENT);
        }

        private void NextActionForcedRecovery(ref DeterminePostMoveStepState token)
        {
            ForcedRecoveryStepState forcedRecoveryToken = new ForcedRecoveryStepState
            {
                PieceMoved = token.PieceMoved,
                PieceCaptured = token.PieceCaptured
            };

            determinePostMoveSequence.Next(this, ref forcedRecoveryToken, (int)StepABC.A);
        }

        private void NextActionForcedRearrangement(ref DeterminePostMoveStepState token)
        {
            ForcedRearrangementStepState forcedRearrangementToken = new ForcedRearrangementStepState
            {
                PieceToRearrange = token.PieceCaptured
            };

            determinePostMoveSequence.Next(this, ref forcedRearrangementToken, (int)StepABC.B);
        }

        private void NextActionTurnEnd()
        {
            var turnEndToken = new TurnEndStepState();
            determinePostMoveSequence.Next(this, ref turnEndToken, (int)StepABC.C);
        }
    }
}
