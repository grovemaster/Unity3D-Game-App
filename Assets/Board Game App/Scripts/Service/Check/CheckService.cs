using Data.Constants.Board;
using Data.Enums.Piece;
using Data.Enums.Piece.OtherMove;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using Data.Enums.Player;
using Data.Piece.Map;
using ECS.EntityView.Board.Tile;
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
        private TurnService turnService = new TurnService();

        public int CalcNumCommanderThreats(PlayerColor commanderColor, IEntitiesDB entitiesDB)
        {
            return destinationTileService.CalcNumCommanderThreats(commanderColor, entitiesDB);
        }

        public bool IsCommanderInCheck(PlayerColor turnPlayer, IEntitiesDB entitiesDB)
        {
            return destinationTileService.CalcNumCommanderThreats(turnPlayer, entitiesDB) > 0;
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

        public bool CannotCaptureCommanderViolated(Vector2 location, PlayerColor currentTurnColor, IEntitiesDB entitiesDB)
        {
            List<PieceEV> towerPieces = pieceFindService.FindPiecesByLocation(location, entitiesDB);

            return CannotCaptureCommanderViolated(towerPieces, currentTurnColor, entitiesDB);
        }

        public bool CannotCaptureCommanderViolated(List<PieceEV> towerPieces, PlayerColor currentTurnColor, IEntitiesDB entitiesDB)
        {
            List<PieceEV> newTowerPieces = towerPieces.Where(piece => piece.Location.Location != BoardConst.HAND_LOCATION).ToList();

            return newTowerPieces.Count > 1
                && newTowerPieces[newTowerPieces.Count - 2].PlayerOwner.PlayerColor != currentTurnColor
                && newTowerPieces[newTowerPieces.Count - 2].Piece.PieceType == PieceType.COMMANDER
                && AbilityToPiece.HasAbility(PreMoveAbility.CANNOT_CAPTURE_COMMANDER, newTowerPieces.Last().Piece.PieceType)
                && FindCheckmateFoulThreatsToLocation(newTowerPieces[0].Location.Location, currentTurnColor, entitiesDB).Count > 0
                && FindThreatsToLocation(newTowerPieces[0].Location.Location, currentTurnColor, entitiesDB).Count == 0;
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

        #region Substitution
        public bool IsSubstitutionPossible(PieceEV? clickedPiece, TileEV? clickedTile, IEntitiesDB entitiesDB)
        {
            // Remember, Commander cannot run to Samurai during check, b/c that's a violation of a different rule
            /* Conditions:
             *      piece clicked
             *      not a destination tile (user is clicking this piece to access substitution ability or click-highlight it)
             *      commander in check
             *      samurai (has substitution ability)
             *      piece is topOfTower
             *      piece tier == 1
             *      piece is adjacent to Commander vertically or horizontally
             *      substitution would resolve check
             */

            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            PieceEV commander = pieceFindService.FindCommander(currentTurn.TurnPlayer.PlayerColor, entitiesDB);

            return currentTurn.Check.CommanderInCheck
                && clickedPiece.HasValue
                && clickedTile.HasValue
                && (!clickedTile.Value.Tile.PieceRefEntityId.HasValue || clickedTile.Value.Tile.PieceRefEntityId.Value == 0)
                && AbilityToPiece.HasAbility(OtherMoveAbility.SUBSTITUTION, clickedPiece.Value.Piece.PieceType)
                && clickedPiece.Value.Tier.Tier == 1
                && clickedPiece.Value.Tier.TopOfTower
                && IsAdjacentLocationToCommander(clickedPiece.Value, commander)
                && DoesSubstitutionResolveCheck(clickedPiece.Value, commander, entitiesDB);
        }

        private bool IsAdjacentLocationToCommander(PieceEV piece, PieceEV commander)
        {
            return (piece.Location.Location.x == commander.Location.Location.x || piece.Location.Location.y == commander.Location.Location.y)
                && (Math.Abs(piece.Location.Location.x - commander.Location.Location.x) == 1 || Math.Abs(piece.Location.Location.y - commander.Location.Location.y) == 1);
        }

        private bool DoesSubstitutionResolveCheck(PieceEV ninja, PieceEV commander, IEntitiesDB entitiesDB)
        {
            Vector2 commanderLocation = new Vector2(commander.Location.Location.x, commander.Location.Location.y);
            int commanderTier = commander.Tier.Tier;
            bool commanderTopOfTower = commander.Tier.TopOfTower;

            pieceSetService.SetPieceLocationAndTier(commander, ninja.Location.Location, ninja.Tier.Tier, entitiesDB);
            pieceSetService.SetTopOfTower(commander, entitiesDB, ninja.Tier.TopOfTower);

            pieceSetService.SetPieceLocationAndTier(ninja, commanderLocation, commanderTier, entitiesDB);
            pieceSetService.SetTopOfTower(ninja, entitiesDB, commanderTopOfTower);

            // Make check
            bool returnValue = !IsCommanderInCheck(commander.PlayerOwner.PlayerColor, entitiesDB);

            pieceSetService.SetPieceLocationAndTier(ninja, commander.Location.Location, commander.Tier.Tier, entitiesDB);
            pieceSetService.SetTopOfTower(ninja, entitiesDB, commander.Tier.TopOfTower);

            pieceSetService.SetPieceLocationAndTier(commander, commanderLocation, commanderTier, entitiesDB);
            pieceSetService.SetTopOfTower(commander, entitiesDB, commanderTopOfTower);

            return returnValue;
        }
        #endregion

        private bool EnemyThreatensCommanderFromAdjacentTier(PieceEV commander, PieceEV enemyPiece)
        {
            return commander.Location.Location == enemyPiece.Location.Location
                && Math.Abs(commander.Tier.Tier - enemyPiece.Tier.Tier) == 1;
        }

        private List<PieceEV> FindThreatsToLocation(Vector2 location, PlayerColor playerColor, IEntitiesDB entitiesDB)
        {
            List<PieceEV> returnValue = new List<PieceEV>();
            List<PieceEV> teamPieces = pieceFindService.FindPiecesByTeam(playerColor, entitiesDB).ToList();
            List<PieceEV> allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();

            foreach (PieceEV teamPiece in teamPieces)
            {
                List<Vector2> destinations = destinationTileService.CalcDestinationTileLocations(teamPiece, entitiesDB, allPieces, false);

                if (destinations.Contains(location))
                {
                    returnValue.Add(teamPiece);
                }
            }

            return returnValue;
        }

        private List<PieceEV> FindCheckmateFoulThreatsToLocation(Vector2 location, PlayerColor teamColor, IEntitiesDB entitiesDB)
        {
            List<PieceEV> allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();

            // Cannot stack 2 pieces of same type and team in a tower, BUT we're doing predictive modeling:
            // A topOfTower Bronze checkmates Commander underneath while there's an adjacent Bronze to capture Commander on next turn
            // (and no other pieces to also capture Commander)
            return pieceFindService.FindPiecesByTeamAndAbility(PreMoveAbility.CANNOT_CAPTURE_COMMANDER, teamColor, entitiesDB).Where(piece =>
                destinationTileService.CalcSingleDestinationTileLocationsWithoutFullAdjustment(piece, allPieces, entitiesDB).Contains(location)).ToList();
        }
    }
}
