using Data.Step.Piece.Ability.Substitution;
using Data.Step.Turn;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Piece.Find;
using Service.Piece.Set;
using Service.Turn;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Engine.Piece.Ability.Substitution
{
    class SubstitutionEngine : IStep<SubstitutionStepState>, IQueryingEntitiesEngine
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private TurnService turnService = new TurnService();

        private readonly ISequencer substitutionSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public SubstitutionEngine(ISequencer substitutionSequence)
        {
            this.substitutionSequence = substitutionSequence;
        }

        public void Ready()
        { }

        public void Step(ref SubstitutionStepState token, int condition)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            PieceEV commander = pieceFindService.FindCommander(currentTurn.TurnPlayer.PlayerColor, entitiesDB);
            PieceEV ninja = token.SubstitutionPiece;

            SwitchCommanderAndNinjaPiece(commander, ninja);

            NextActionTurnEnd();
        }

        private void SwitchCommanderAndNinjaPiece(PieceEV commander, PieceEV ninja)
        {
            Vector2 newNinjaLocation = new Vector2(commander.Location.Location.x, commander.Location.Location.y);
            int newNinjaTier = commander.Tier.Tier;
            bool newNinjaTopOfTower = commander.Tier.TopOfTower;

            pieceSetService.SetPieceLocationAndTier(commander, ninja.Location.Location, ninja.Tier.Tier, entitiesDB);
            pieceSetService.SetTopOfTower(commander, entitiesDB, ninja.Tier.TopOfTower);

            pieceSetService.SetPieceLocationAndTier(ninja, newNinjaLocation, newNinjaTier, entitiesDB);
            pieceSetService.SetTopOfTower(ninja, entitiesDB, newNinjaTopOfTower);

            commander.MovePiece.NewLocation = commander.Location.Location;
            ninja.MovePiece.NewLocation = ninja.Location.Location;
        }

        private void NextActionTurnEnd()
        {
            var turnEndToken = new TurnEndStepState();
            substitutionSequence.Next(this, ref turnEndToken);
        }
    }
}
