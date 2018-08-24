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
            int returnValue = 0;
            PieceEV commander = pieceFindService.FindCommander(commanderColor, entitiesDB);
            List<PieceEV> allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();
            List<PieceEV> commanderTowerPieces = pieceFindService.FindPiecesByLocation(commander.Location.Location, entitiesDB);

            if (destinationTileService.IsCommanderBuried(commander, commanderTowerPieces))
            {
                // Commander cannot be captured this turn
                return returnValue;
            }

            if (destinationTileService.IsCommanderInDangerFromBelow(commander, commanderTowerPieces))
            {
                returnValue++;
            }

            if (commander.Tier.TopOfTower)
            {
                List<PieceEV> enemyPieces = allPieces.Where(piece =>
                    piece.PlayerOwner.PlayerColor != commanderColor && piece.Tier.TopOfTower).ToList();

                foreach (PieceEV enemy in enemyPieces)
                {
                    if (destinationTileService.CalcDestinationTileLocations(enemy, entitiesDB, allPieces, false).Contains(commander.Location.Location))
                    {
                        returnValue++;
                    }
                }
            }

            return returnValue;
        }

        public bool IsCommanderInCheck(PlayerColor turnPlayer, IEntitiesDB entitiesDB)
        {
            return CalcNumCommanderThreats(turnPlayer, entitiesDB) > 0;
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
