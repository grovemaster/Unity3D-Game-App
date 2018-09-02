using Data.Enums.Piece;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.Side;
using Data.Enums.Player;
using Data.Piece.Map;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Board;
using Service.Piece.Find;
using Service.Piece.Set;
using Service.Turn;
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

        public bool DropReleasesCheck(
            PieceEV pieceToDrop,
            Vector2 location,
            TurnEV turn,
            PieceSide side,
            IEntitiesDB entitiesDB)
        {
            bool returnValue = false;

            PieceEV? topPieceAtLocation = pieceFindService.FindTopPieceByLocation(location, entitiesDB);
            pieceSetService.SetTopOfTowerToFalse(topPieceAtLocation, entitiesDB);

            int tier = topPieceAtLocation.HasValue ? topPieceAtLocation.Value.Tier.Tier + 1 : 1;
            pieceSetService.SetPieceLocationAndTier(pieceToDrop, location, tier, entitiesDB);
            pieceSetService.SetPiecePlayerOwner(pieceToDrop, turn.TurnPlayer.PlayerColor, entitiesDB);
            pieceSetService.SetPieceSide(pieceToDrop, side, entitiesDB);

            returnValue = !IsCommanderInCheck(turn.TurnPlayer.PlayerColor, entitiesDB);

            pieceSetService.SetPieceLocationToHandLocation(pieceToDrop, entitiesDB);

            if (topPieceAtLocation.HasValue)
            {
                pieceSetService.SetTopOfTower(topPieceAtLocation.Value, entitiesDB);
            }

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

        public bool DoesLowerTierThreatenCommander(
            PieceEV commander, PieceEV enemyPiece, PieceEV secondFromTopEnemyPiece, List<PieceEV> towerPieces, IEntitiesDB entitiesDB)
        {
            // If middle piece is lance, Forced Rearrangement could potentially resolve or prevent check
            // NOTE: Below foreach loop causes invalid tier values, I'm just too lazy too deal with it right now
            foreach(PieceEV piece in towerPieces)
            {
                piece.Tier.Tier--;
            }

            bool returnValue = commander.Tier.TopOfTower
                && destinationTileService.CalcDestinationTileLocations(enemyPiece, entitiesDB, null, false).Contains(commander.Location.Location);

            if (returnValue && IsForcedRearrangementPossible(secondFromTopEnemyPiece))
            {
                returnValue = DoesForcedRearrangementResolveOrPreventCheck(
                    secondFromTopEnemyPiece, commander, towerPieces, entitiesDB);
            }

            foreach (PieceEV piece in towerPieces)
            {
                piece.Tier.Tier++;
            }

            return returnValue;
        }

        #region Forced Rearrangement
        public bool DoesForcedRearrangementResolveOrPreventCheck(
            PieceEV forcedRearrangementPiece, PieceEV commander, List<PieceEV> towerPieces, IEntitiesDB entitiesDB)
        {
            List <PieceEV> allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();
            List<PieceEV> enemyPieces = pieceFindService.FindPiecesByTeam(
                TurnService.CalcOtherTurnPlayer(commander.PlayerOwner.PlayerColor), entitiesDB).ToList();
            List<PieceEV> actualThreats = destinationTileService.FindActualEnemyThreats(
                commander, enemyPieces, allPieces, entitiesDB);

            return destinationTileService.ForcedRearrangementCanResolveThreats(
                commander, forcedRearrangementPiece, actualThreats, allPieces, entitiesDB);
        }

        // TODO This public method belongs in a different service, such as a PieceAbilityService
        public bool IsForcedRearrangementPossible(PieceEV forcedRearrangementPiece)
        {
            return AbilityToPiece.HasAbility(PostMoveAbility.FORCED_REARRANGEMENT, forcedRearrangementPiece.Piece.PieceType);
        }
        #endregion

        private bool EnemyThreatensCommanderFromAdjacentTier(PieceEV commander, PieceEV enemyPiece)
        {
            return commander.Location.Location == enemyPiece.Location.Location
                && Math.Abs(commander.Tier.Tier - enemyPiece.Tier.Tier) == 1;
        }
    }
}
