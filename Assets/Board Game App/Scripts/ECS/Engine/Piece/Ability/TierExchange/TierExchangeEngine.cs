using Data.Step.Piece.Ability.TierExchange;
using Data.Step.Turn;
using ECS.EntityView.Piece;
using Service.Piece.Find;
using Service.Piece.Set;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Piece.Ability.TierExchange
{
    class TierExchangeEngine : IStep<TierExchangeStepState>, IQueryingEntitiesEngine
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private TurnService turnService = new TurnService();

        private readonly ISequencer tierExchangeSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public TierExchangeEngine(ISequencer tierExchangeSequence)
        {
            this.tierExchangeSequence = tierExchangeSequence;
        }

        public void Ready()
        { }

        public void Step(ref TierExchangeStepState token, int condition)
        {
            SwitchBottomAndTopTowerPieces(token.ReferenceTile.Location.Location);

            NextActionTurnEnd();
        }

        private void SwitchBottomAndTopTowerPieces(Vector2 towerLocation)
        {
            List<PieceEV> towerPieces = pieceFindService.FindPiecesByLocation(towerLocation, entitiesDB);

            // Shouldn't be here if tower is not size of 3
            PieceEV tier1Piece = towerPieces[0];
            PieceEV tier3Piece = towerPieces[2];

            int newTier1Tier = tier1Piece.Tier.Tier;
            bool newTier1TopOfTower = tier1Piece.Tier.TopOfTower;

            pieceSetService.SetPieceLocationAndTier(tier1Piece, tier1Piece.Location.Location, tier3Piece.Tier.Tier, entitiesDB);
            pieceSetService.SetTopOfTower(tier1Piece, entitiesDB, tier3Piece.Tier.TopOfTower);

            pieceSetService.SetPieceLocationAndTier(tier3Piece, tier3Piece.Location.Location, newTier1Tier, entitiesDB);
            pieceSetService.SetTopOfTower(tier3Piece, entitiesDB, newTier1TopOfTower);

            tier1Piece.MovePiece.NewLocation = tier1Piece.Location.Location;
            tier3Piece.MovePiece.NewLocation = tier3Piece.Location.Location;
        }

        private void NextActionTurnEnd()
        {
            var turnEndToken = new TurnEndStepState();
            tierExchangeSequence.Next(this, ref turnEndToken);
        }
    }
}
