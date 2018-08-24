using Data.Enum.Player;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Board;
using Service.Piece.Find;
using Service.Piece.Set;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Check
{
    public class CheckService
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private DestinationTileService destinationTileService = new DestinationTileService();

        public int CalcNumCommanderThreats(PlayerColor commanderColor, IEntitiesDB entitiesDB)
        {
            return destinationTileService.CalcNumCommanderThreats(commanderColor, entitiesDB);
        }

        public bool IsCommanderInCheck(PlayerColor turnPlayer, IEntitiesDB entitiesDB)
        {
            return destinationTileService.CalcNumCommanderThreats(turnPlayer, entitiesDB) > 0;
        }

        public bool DropReleasesCheck(PieceEV pieceToDrop, Vector2 location, TurnEV turn, IEntitiesDB entitiesDB)
        {
            bool returnValue = false;

            pieceSetService.SetPieceLocationAndTier(pieceToDrop, location, 1, entitiesDB);
            pieceSetService.SetPiecePlayerOwner(pieceToDrop, turn.TurnPlayer.PlayerColor, entitiesDB);

            returnValue = !IsCommanderInCheck(turn.TurnPlayer.PlayerColor, entitiesDB);

            pieceSetService.SetPieceLocationToHandLocation(pieceToDrop, entitiesDB);

            return returnValue;
        }

        public bool ForcedRecoveryResolvesOrDoesNotCreateCheck(PieceEV pieceMoved, TurnEV turn, IEntitiesDB entitiesDB)
        {
            bool returnValue = false;
            Vector2 oldLocation = new Vector2(pieceMoved.Location.Location.x, pieceMoved.Location.Location.y);
            int oldTier = pieceMoved.Tier.Tier;
            bool oldTopOfTower = pieceMoved.Tier.TopOfTower;

            pieceSetService.SetPieceLocationToHandLocation(pieceMoved, entitiesDB);

            returnValue = !IsCommanderInCheck(turn.TurnPlayer.PlayerColor, entitiesDB);

            pieceSetService.SetPieceLocationAndTier(pieceMoved, oldLocation, oldTier, entitiesDB);
            pieceSetService.SetTopOfTower(pieceMoved, entitiesDB, oldTopOfTower);

            return returnValue;
        }

        public bool DoesTopOfTowerThreatenCommander(PieceEV commander, PieceEV enemyPiece, IEntitiesDB entitiesDB)
        {
            return (commander.Tier.TopOfTower
                && destinationTileService.CalcDestinationTileLocations(enemyPiece, entitiesDB).Contains(commander.Location.Location))
                || (!commander.Tier.TopOfTower && EnemyThreatensCommanderFromAdjacentTier(commander, enemyPiece));
        }

        public bool DoesLowerTierThreatenCommander(PieceEV commander, PieceEV enemyPiece, IEntitiesDB entitiesDB)
        {
            enemyPiece.Tier.Tier--;

            bool returnValue = commander.Tier.TopOfTower
                && destinationTileService.CalcDestinationTileLocations(enemyPiece, entitiesDB, null, false).Contains(commander.Location.Location);

            enemyPiece.Tier.Tier++;

            return returnValue;
        }

        private bool EnemyThreatensCommanderFromAdjacentTier(PieceEV commander, PieceEV enemyPiece)
        {
            return commander.Location.Location == enemyPiece.Location.Location
                && Math.Abs(commander.Tier.Tier - enemyPiece.Tier.Tier) == 1;
        }
    }
}
