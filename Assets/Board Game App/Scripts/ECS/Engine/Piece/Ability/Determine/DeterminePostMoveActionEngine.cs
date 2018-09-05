using Data.Enums.Piece;
using Data.Enums.Piece.PostMove;
using Data.Piece.Map;
using Data.Step.Piece.Ability;
using Data.Step.Piece.Ability.Betrayal;
using Data.Step.Piece.Ability.ForcedRearrangement;
using Data.Step.Piece.Move;
using Data.Step.Turn;
using Svelto.ECS;

namespace ECS.Engine.Piece.Ability.Determine
{
    class DeterminePostMoveActionEngine : IStep<DeterminePostMoveStepState>, IQueryingEntitiesEngine
    {
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
            if (BetrayalInEffect(ref token))
            {
                NextActionBetrayal(ref token);
            }
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

        // TODO This is duplicated elsewhere, refactor into common service later
        private bool BetrayalInEffect(ref DeterminePostMoveStepState token)
        {
            return token.PieceMoved.Tier.TopOfTower
                && token.PieceCaptured.HasValue
                && AbilityToPiece.HasAbility(PostMoveAbility.BETRAYAL, token.PieceMoved.Piece.PieceType);
        }

        private bool HasForcedRecoveryAbility(PieceType pieceType)
        {
            return AbilityToPiece.HasAbility(PostMoveAbility.FORCED_RECOVERY, pieceType);
        }

        private bool HasForcedRearrangementAbility(PieceType pieceType)
        {
            return AbilityToPiece.HasAbility(PostMoveAbility.FORCED_REARRANGEMENT, pieceType);
        }

        private void NextActionBetrayal(ref DeterminePostMoveStepState token)
        {
            BetrayalStepState betrayalToken = new BetrayalStepState
            {
                PieceMoved = token.PieceMoved,
                PieceCaptured = token.PieceCaptured
            };

            determinePostMoveSequence.Next(this, ref betrayalToken, (int)PostMoveState.BETRAYAL);
        }

        private void NextActionForcedRecovery(ref DeterminePostMoveStepState token)
        {
            ForcedRecoveryStepState forcedRecoveryToken = new ForcedRecoveryStepState
            {
                PieceMoved = token.PieceMoved,
                PieceCaptured = token.PieceCaptured
            };

            determinePostMoveSequence.Next(this, ref forcedRecoveryToken, (int)PostMoveState.FORCED_RECOVERY);
        }

        private void NextActionForcedRearrangement(ref DeterminePostMoveStepState token)
        {
            ForcedRearrangementStepState forcedRearrangementToken = new ForcedRearrangementStepState
            {
                PieceToRearrange = token.PieceCaptured
            };

            determinePostMoveSequence.Next(this, ref forcedRearrangementToken, (int)PostMoveState.FORCED_REARRANGEMENT);
        }

        private void NextActionTurnEnd()
        {
            var turnEndToken = new TurnEndStepState();
            determinePostMoveSequence.Next(this, ref turnEndToken, (int)PostMoveState.TURN_END);
        }
    }
}
