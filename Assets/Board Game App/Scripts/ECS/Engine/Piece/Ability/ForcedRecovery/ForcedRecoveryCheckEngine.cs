using System;
using Data.Enum.AB;
using Data.Enum.Piece.PostMove;
using Data.Piece;
using Data.Step.Piece.Move;
using Data.Step.Turn;
using ECS.EntityView.Piece;
using Service.Board;
using Service.Piece.Factory;
using Svelto.ECS;

namespace ECS.Engine.Piece.Ability.ForcedRecovery
{
    class ForcedRecoveryCheckEngine : IStep<ForcedRecoveryStepState>, IQueryingEntitiesEngine
    {
        private DestinationTileService destinationTileService = new DestinationTileService();
        private PieceFactory pieceFactory = new PieceFactory();

        private readonly ISequencer forcedRecoverySequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public ForcedRecoveryCheckEngine(ISequencer forcedRecoverySequence)
        {
            this.forcedRecoverySequence = forcedRecoverySequence;
        }

        public void Ready()
        { }

        public void Step(ref ForcedRecoveryStepState token, int condition)
        {
            IPieceData piece = pieceFactory.CreateIPieceData(token.pieceMoved.Piece.PieceType);

            bool forcedRecoveryPossible = token.pieceMoved.Tier.TopOfTower // Paranoia check
                && piece.Abilities.PostMove.HasValue && piece.Abilities.PostMove.Value == PostMoveAbility.FORCED_RECOVERY
                && !HasDestinationTiles(token.pieceMoved);

            NextAction(ref token, forcedRecoveryPossible);
        }

        private bool HasDestinationTiles(PieceEV piece)
        {
            return destinationTileService.CalcDestinationTileLocations(piece, entitiesDB).Count > 0;
        }

        private void NextAction(ref ForcedRecoveryStepState token, bool forcedRecoveryPossible)
        {
            forcedRecoverySequence.Next(this, ref token, (int)(forcedRecoveryPossible ? StepAB.A : StepAB.B));
        }

        private void NextActionTurnEnd()
        {
            var turnEndToken = new TurnEndStepState();
            forcedRecoverySequence.Next(this, ref turnEndToken);
        }
    }
}
