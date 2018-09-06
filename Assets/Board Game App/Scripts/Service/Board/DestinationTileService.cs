using Data.Check.PreviousMove;
using Data.Constants.Board;
using Data.Enums.Piece;
using Data.Enums.Piece.PostMove;
using Data.Enums.Piece.PreMove;
using Data.Enums.Player;
using Data.Piece;
using Data.Piece.Map;
using ECS.EntityView.Piece;
using Service.Distance;
using Service.Drop;
using Service.Piece.Factory;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Board
{
    public class DestinationTileService
    {
        private DistanceService distanceService = new DistanceService();
        private PieceFactory pieceFactory = new PieceFactory();
        private PieceFindService pieceFindService = new PieceFindService();
        private PreDropService preDropService = new PreDropService();
        private TurnService turnService = new TurnService();

        #region Public API
        /**
         * This will return values exceeding board boundaries (such as below the zero-th rank).  It is the
         * responsibility of the client to not abuse this data.
         */
        public List<Vector2> CalcDestinationTileLocations(
            PieceEV pieceEV, IEntitiesDB entitiesDB, List<PieceEV> allPieces = null, bool excludeCheckViolations = true)
        {
            List<Vector2> returnValue = new List<Vector2>();

            if (allPieces == null)
            {
                // Current piece included, doesn't matter for current calc
                allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();
            }

            returnValue.AddRange(CalcDestinations(pieceEV, allPieces, entitiesDB, excludeCheckViolations));

            return returnValue;
        }

        public HashSet<Vector2> CalcDestinationTileLocations(PieceEV[] pieces, bool excludeCheckViolations, IEntitiesDB entitiesDB)
        {
            HashSet<Vector2> returnValue = new HashSet<Vector2>();
            List<PieceEV> allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();

            for (int i = 0; i < pieces.Length; ++i)
            {
                returnValue.UnionWith(CalcDestinationTileLocations(pieces[i], entitiesDB, allPieces, excludeCheckViolations));
            }

            return returnValue;
        }

        #region In Check
        public int CalcNumCommanderThreats(PlayerColor commanderColor, IEntitiesDB entitiesDB)
        {
            int returnValue = 0;
            PieceEV commander = pieceFindService.FindCommander(commanderColor, entitiesDB);
            List<PieceEV> allPieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();
            List<PieceEV> commanderTowerPieces = pieceFindService.FindPiecesByLocation(commander.Location.Location, entitiesDB);

            if (IsCommanderBuried(commander, commanderTowerPieces))
            {
                // Commander cannot be captured this turn
                return returnValue;
            }

            if (IsCommanderInDangerFromBelow(commander, commanderTowerPieces))
            {
                returnValue++;
            }

            if (commander.Tier.TopOfTower)
            {
                List<PieceEV> enemyPieces = allPieces.Where(piece =>
                    piece.PlayerOwner.PlayerColor != commanderColor && piece.Tier.TopOfTower).ToList();

                foreach (PieceEV enemy in enemyPieces)
                {
                    if (CalcDestinationTileLocations(enemy, entitiesDB, allPieces, false).Contains(commander.Location.Location))
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
        #endregion

        #region Small Commander Checks
        public bool IsCommanderBuried(PieceEV commander, List<PieceEV> commanderTowerPieces)
        {
            return !commander.Tier.TopOfTower
                && !IsAdjacentPieceEnemy(commander, commanderTowerPieces);
        }

        public bool IsCommanderInDangerFromBelow(PieceEV commander, List<PieceEV> commanderTowerPieces)
        {
            return commander.Tier.Tier > 1
                && commander.PlayerOwner.PlayerColor != commanderTowerPieces[commander.Tier.Tier - 2].PlayerOwner.PlayerColor;
        }
        #endregion

        #region Find Enemy Threats
        public List<PieceEV> FindActualEnemyThreats(
            PieceEV commander, List<PieceEV> enemyThreats, List<PieceEV> allPieces, IEntitiesDB entitiesDB)
        {
            List<PieceEV> returnValue = new List<PieceEV>();

            foreach (PieceEV threat in enemyThreats)
            {
                if (threat.Tier.TopOfTower // Temporarily moved piece may have captured/stacked this enemy piece
                    && (threat.Location.Location == commander.Location.Location
                    || CalcDestinations(threat, allPieces, entitiesDB, false).Contains(commander.Location.Location)))
                {
                    returnValue.Add(threat);
                }
            }

            return returnValue;
        }
        #endregion

        #region Forced Rearrangement
        public bool ForcedRearrangementCanResolveThreats(
            PieceEV commander, PieceEV pieceToDrop, List<PieceEV> actualThreats, List<PieceEV> allPieces, IEntitiesDB entitiesDB)
        {
            bool returnValue = false;

            // Test drop each valid location to see if threats resolved
            for (int rank = turnService.GetMinRankWithinTerritory(commander.PlayerOwner.PlayerColor); rank <= turnService.GetMaxRankWithinTerritory(commander.PlayerOwner.PlayerColor); ++rank)
            {
                for (int file = 0; file < BoardConst.NUM_FILES_RANKS; ++file)
                {
                    Vector2 location = new Vector2(file, rank);
                    List<PieceEV> piecesAtLocation = pieceFindService.FindPiecesByLocation(location, entitiesDB);

                    if (IsValidDrop(piecesAtLocation, entitiesDB)
                        && DoesDropResolveCheck(
                            commander, pieceToDrop, location, piecesAtLocation, actualThreats, allPieces, entitiesDB))
                    {
                        returnValue = true;
                        break;
                    }
                }
            }

            return returnValue;
        }
        #endregion
        #endregion

        private List<Vector2> CalcDestinations(
            PieceEV pieceEV,
            List<PieceEV> allPieces,
            IEntitiesDB entitiesDB,
            bool excludeCheckViolations = false,
            bool excludeObstructedDestinations = true)
        {
            bool useGoldMovement = IsOpponentPieceDirectlyBelow(pieceEV, allPieces);
            IPieceData pieceData = CreatePieceData(pieceEV, useGoldMovement);

            List<Vector2> returnValue = AdjustAndExcludeSingleDestinations(
                pieceData,
                pieceEV,
                allPieces,
                excludeObstructedDestinations);

            returnValue.AddRange(AdjustAndExcludeJumpDestinations(
                pieceData,
                pieceEV,
                allPieces,
                excludeObstructedDestinations
                ));

            returnValue.AddRange(AdjustAndExcludeLineDestinations(
                pieceData,
                pieceEV,
                allPieces,
                excludeObstructedDestinations));

            if (excludeCheckViolations) // Should only happen for turn player
            {
                ExcludeCheckViolations(pieceEV, returnValue, allPieces, entitiesDB);
            }

            return returnValue;
        }
        #region Create Piece Data
        private IPieceData CreatePieceData(PieceEV pieceEV, bool useGoldMovement)
        {
            PieceType pieceToCreate = !useGoldMovement ? pieceEV.Piece.PieceType : PieceType.GOLD;

            return pieceFactory.CreateIPieceData(pieceToCreate);
        }
        #endregion

        #region Adjust, & Exclude
        private List<Vector2> AdjustAndExcludeSingleDestinations(
            IPieceData pieceData,
            PieceEV pieceEV,
            List<PieceEV> allPieces,
            bool excludeObstructedDestinations)
        {
            int tier = CalcTierToUse(pieceEV, allPieces);
            List<Vector2> returnValue = pieceData.Tiers[tier - 1].Single;
            AdjustDestinations(returnValue, pieceEV, allPieces);

            if (excludeObstructedDestinations) // Do NOT allow destinations other pieces in the way
            {
                ExcludeDestinationsWithObstructingPieces(pieceEV, returnValue, allPieces, true);
            }

            return returnValue;
        }

        private List<Vector2> AdjustAndExcludeJumpDestinations(
            IPieceData pieceData,
            PieceEV pieceEV,
            List<PieceEV> allPieces,
            bool excludeObstructedDestinations)
        {
            int tier = CalcTierToUse(pieceEV, allPieces);
            List<Vector2> returnValue = pieceData.Tiers[tier - 1].Jump;
            AdjustDestinations(returnValue, pieceEV, allPieces);

            if (excludeObstructedDestinations)
            {
                ExcludeJumpDestinationsWithObstructingMrePieces(pieceEV, returnValue, allPieces);
            }

            return returnValue;
        }

        private IEnumerable<Vector2> AdjustAndExcludeLineDestinations(
            IPieceData pieceData,
            PieceEV pieceEV,
            List<PieceEV> allPieces,
            bool excludeObstructedDestinations)
        {
            int tier = CalcTierToUse(pieceEV, allPieces);
            List<Vector2> vectors = pieceData.Tiers[tier - 1].Line;
            List<Vector2> returnValue = new List<Vector2>();

            foreach (Vector2 vector in vectors)
            {
                List<Vector2> line = ExpandLineVector(vector);
                AdjustRawDataWithPieceLocationAndDirection(pieceEV, line);
                ExcludeOutOfBoard(line);

                if (excludeObstructedDestinations) // Do NOT allow destinations other pieces in the way
                {
                    ExcludeDestinationsWithObstructingPieces(pieceEV, line, allPieces);
                }

                // Always exclude these scenarios
                ExcludeDestinationsAtCannotBeStackedPieces(pieceEV.PlayerOwner.PlayerColor, line, allPieces);
                ExcludeTowerDestinationsWithSameTypeAndTeam(pieceEV, line, allPieces);

                returnValue.AddRange(line);
            }

            return returnValue;
        }

        private void AdjustDestinations(
            List<Vector2> destinations,
            PieceEV pieceEV,
            List<PieceEV> allPieces)
        {
            AdjustRawDataWithPieceLocationAndDirection(pieceEV, destinations);
            ExcludeOutOfBoard(destinations);
            ExcludeDestinationsWithFriendlyTier3Tower(pieceEV, destinations, allPieces);
            ExcludeTowerDestinationsWithSameTypeAndTeam(pieceEV, destinations, allPieces);
            ExcludeTwoFileMoveViolations(pieceEV, destinations, allPieces);
            ExcludeDestinationsAtCannotBeStackedPieces(pieceEV.PlayerOwner.PlayerColor, destinations, allPieces);
        }

        #region Line
        private List<Vector2> ExpandLineVector(Vector2 vector)
        {
            List<Vector2> returnValue = new List<Vector2>();
            returnValue.Add(new Vector2(vector.x, vector.y));

            for (int i = 1; i < BoardConst.NUM_FILES_RANKS; ++i)
            {
                returnValue.Add(new Vector2(
                    returnValue[returnValue.Count - 1].x + vector.x,
                    returnValue[returnValue.Count - 1].y + vector.y
                    ));
            }

            return returnValue;
        }
        #endregion
        #endregion

        #region Excludes
        private bool IsOpponentPieceDirectlyBelow(PieceEV pieceEV, List<PieceEV> allPieces)
        {
            return pieceEV.Tier.Tier != 1
                && allPieces.Where(piece =>
                    piece.Location.Location == pieceEV.Location.Location
                    && piece.PlayerOwner.PlayerColor != pieceEV.PlayerOwner.PlayerColor
                    && piece.Tier.Tier + 1 == pieceEV.Tier.Tier)
                .Count() > 0;
        }

        private void AdjustRawDataWithPieceLocationAndDirection(
            PieceEV pieceEV, List<Vector2> rawLocationData)
        {
            // Add piece's location to value
            for (int i = 0; i < rawLocationData.Count; ++i)
            {
                rawLocationData[i] = new Vector2(
                    pieceEV.Location.Location.x + (rawLocationData[i].x * (int)pieceEV.Piece.Direction),
                    pieceEV.Location.Location.y + (rawLocationData[i].y * (int)pieceEV.Piece.Direction));
            }
        }

        /**
         * If destinations are outside board, such as (-2, 1), then exclude them.
         */
        private void ExcludeOutOfBoard(List<Vector2> returnValue)
        {
            returnValue.RemoveAll(location =>
                location.x < 0 || location.x >= BoardConst.NUM_FILES_RANKS
                || location.y < 0 || location.y >= BoardConst.NUM_FILES_RANKS
            );
        }

        /**
         * If destination contains a tier 3 tower with a friendly piece on top,
         * then that tile is not a valid destination tile.
         */
        private void ExcludeDestinationsWithFriendlyTier3Tower(
            PieceEV pieceToCalc, List<Vector2> destinations, List<PieceEV> allPieces)
        {
            List<Vector2> destinationsToRemove = new List<Vector2>();

            foreach (Vector2 destination in destinations)
            {
                if (HasFriendlyTier3Tower(pieceToCalc, destination, allPieces))
                {
                    destinationsToRemove.Add(destination);
                }
            }

            foreach (Vector2 removeDestination in destinationsToRemove)
            {
                destinations.Remove(removeDestination);
            }
        }

        private void ExcludeTowerDestinationsWithSameTypeAndTeam(
            PieceEV pieceToCalc, List<Vector2> destinations, List<PieceEV> allPieces)
        {
            List<Vector2> destinationsToRemove = new List<Vector2>();

            foreach (Vector2 destination in destinations)
            {
                if (HasSameTypeAndTeamInTower(pieceToCalc, destination, allPieces))
                {
                    destinationsToRemove.Add(destination);
                }
            }

            foreach (Vector2 removeDestination in destinationsToRemove)
            {
                destinations.Remove(removeDestination);
            }
        }

        /**
         * If pieceEV has betrayal and two file move (Bronze), deny all destination tiles with another friendly piece
         * with same abilities (Bronze)
         * 
         * Also, deny destination if it has an enemy tier 3 tower with a buried enemy pawn
         *      Reason is capture would be mandatory, triggering betrayal, flipping buried enemy pawn into friendly
         *      bronze, thus violating two file move
         */
        private void ExcludeTwoFileMoveViolations(
            PieceEV pieceEV, List<Vector2> destinations, List<PieceEV> allPieces)
        {
            // Only care about Bronze pieces, which only have Single move sets
            if (!AbilityToPiece.HasAbility(PostMoveAbility.BETRAYAL, pieceEV.Piece.PieceType)
                || !AbilityToPiece.HasAbility(PreMoveAbility.TWO_FILE_MOVE, pieceEV.Piece.PieceType))
            {
                return;
            }

            List<Vector2> destinationsToRemove = new List<Vector2>();
            List<PieceEV> otherPiecesOfSameType = allPieces.Where(piece => piece.Piece.PieceType == pieceEV.Piece.PieceType
                && piece.PlayerOwner.PlayerColor == pieceEV.PlayerOwner.PlayerColor
                && piece.ID.entityID != pieceEV.ID.entityID).ToList();

            foreach(Vector2 destination in destinations)
            {
                if (otherPiecesOfSameType.Where(piece => piece.Location.Location.x == destination.x).ToList().Count > 0)
                {
                    destinationsToRemove.Add(destination);
                }
                else
                {
                    List<PieceEV> destinationPieces = FindPiecesAtLocation(destination, allPieces);

                    if (destinationPieces.Count == 3 && destinationPieces[2].PlayerOwner.PlayerColor != pieceEV.PlayerOwner.PlayerColor
                        && ((destinationPieces[0].Piece.PieceType != pieceEV.Piece.PieceType && destinationPieces[0].Piece.Back == pieceEV.Piece.PieceType)
                        || (destinationPieces[0].Piece.PieceType != pieceEV.Piece.PieceType && destinationPieces[0].Piece.Back == pieceEV.Piece.PieceType)))
                    {
                        destinationsToRemove.Add(destination);
                    }
                }
            }

            foreach (Vector2 removeDestination in destinationsToRemove)
            {
                destinations.Remove(removeDestination);
            }
        }

        private void ExcludeDestinationsAtCannotBeStackedPieces(PlayerColor playerColor, List<Vector2> destinations, List<PieceEV> allPieces)
        {
            List<Vector2> destinationsToRemove = new List<Vector2>();
            List<PieceEV> cannotBeStackedPieces = allPieces.Where(piece => piece.PlayerOwner.PlayerColor == playerColor
                && AbilityToPiece.HasAbility(PreMoveAbility.CANNOT_BE_STACKED, piece.Piece.PieceType)).ToList();

            foreach (Vector2 destination in destinations)
            {
                List<PieceEV> piecesAtDestination = FindPiecesAtLocation(destination, allPieces);

                if (piecesAtDestination.Count > 0
                    && piecesAtDestination.Last().PlayerOwner.PlayerColor == playerColor
                    && AbilityToPiece.HasAbility(PreMoveAbility.CANNOT_BE_STACKED, piecesAtDestination.Last().Piece.PieceType))
                {
                    destinationsToRemove.Add(destination);
                }
            }

            foreach (Vector2 removeDestination in destinationsToRemove)
            {
                destinations.Remove(removeDestination);
            }
        }

        private bool HasFriendlyTier3Tower(
            PieceEV pieceToCalc, Vector2 destination, List<PieceEV> allPieces)
        {
            PlayerColor friendlyColor = pieceToCalc.PlayerOwner.PlayerColor;

            int numPiecesBarringPath = allPieces.Where(piece =>
                piece.Tier.Tier == 3
                && piece.PlayerOwner.PlayerColor == friendlyColor
                && destination == piece.Location.Location).Count();

            return numPiecesBarringPath > 0;
        }

        private bool HasSameTypeAndTeamInTower(
            PieceEV pieceToCalc, Vector2 destination, List<PieceEV> allPieces)
        {
            return allPieces.Where(piece =>
                piece.Location.Location == destination
                && piece.PlayerOwner.PlayerColor == pieceToCalc.PlayerOwner.PlayerColor
                && piece.Piece.PieceType == pieceToCalc.Piece.PieceType).Count() > 0;
        }

        /**
         * Rules state piece cannot jump enemy piece that has received Mobile Range Expansion (MRE).
         * Please note certain pieces cannot receive MRE, and thus do not apply.
         * Jumps are either horizontal/vertical/diagonal (single line path) or L-shaped (L-shaped path).
         * The first 3 are simple cases, because 1 or more MRE-affected enemy pieces will block the path.
         * The L-Shaped path has 2 paths and requires 1 or more MRE-affected enemy pieces blocking both paths.
         */
        private void ExcludeJumpDestinationsWithObstructingMrePieces(
            PieceEV pieceToCalc,
            List<Vector2> destinations,
            List<PieceEV> allPieces)
        {
            if (destinations.Count == 0)
            {
                return;
            }

            List<Vector2> destinationsToRemove = new List<Vector2>();
            List<PieceEV> enemeyMreAffectedPieces = FindMobileRangeExpansionAffectedPieces(
                TurnService.CalcOtherTurnPlayer(pieceToCalc.PlayerOwner.PlayerColor), allPieces);

            if (enemeyMreAffectedPieces.Count == 0)
            {
                return;
            }

            foreach (Vector2 destination in destinations)
            {
                if (IsNotLShapedJump(pieceToCalc.Location.Location, destination))
                {
                    if (ShouldRemoveDestination(pieceToCalc, destination, enemeyMreAffectedPieces, true))
                    {
                        destinationsToRemove.Add(destination);
                    }
                }
                else
                {
                    if (ShouldRemoveLShapedJumpDestination(pieceToCalc, destination, enemeyMreAffectedPieces))
                    {
                        destinationsToRemove.Add(destination);
                    }
                }
            }
            //*/

            foreach (Vector2 removeDestination in destinationsToRemove)
            {
                destinations.Remove(removeDestination);
            }
        }

        /**
         * Potentially modify param destinations, by removing locations with piece(s) in the way
         * 
         * Some destinations are in different rank AND file as pieceToCalc's current location
         */
        private void ExcludeDestinationsWithObstructingPieces(
            PieceEV pieceToCalc,
            List<Vector2> destinations,
            List<PieceEV> allPieces,
            bool canIgnoreFriendlyPieces = false)
        {
            List<Vector2> destinationsToRemove = new List<Vector2>();
            /*
             * Types of destinations
             * * One tile away, horizontally, vertically, or diagonally
             * More than one tile away,
             * * Same rank, different file
             * * Same file, different rank
             * * Different rank, different file
             */

            // for loop hopLocations, since that count will often be less than allPiece's count
            foreach (Vector2 destination in destinations)
            {
                if (ShouldRemoveDestination(pieceToCalc, destination, allPieces, canIgnoreFriendlyPieces))
                {
                    destinationsToRemove.Add(destination);
                }
            }

            foreach (Vector2 removeDestination in destinationsToRemove)
            {
                destinations.Remove(removeDestination);
            }
        }

        private bool ShouldRemoveDestination(
            PieceEV pieceToCalc, Vector2 destination, List<PieceEV> allPieces, bool canIgnoreFriendlyPieces)
        {
            bool returnValue = false;
            Vector2 pieceLocation = pieceToCalc.Location.Location;

            Vector2 increment = new Vector2(
                pieceLocation.x == destination.x ?
                    0 : (pieceLocation.x - destination.x) / Math.Abs(pieceLocation.x - destination.x),
                pieceLocation.y == destination.y ?
                    0 : (pieceLocation.y - destination.y) / Math.Abs(pieceLocation.y - destination.y)
                );

            Vector2 evalLocation = pieceLocation - increment;

            while (evalLocation != destination)
            {
                int numPiecesBarringPath = allPieces.Where(piece =>
                    evalLocation == piece.Location.Location
                    && (!canIgnoreFriendlyPieces
                        || (piece.PlayerOwner.PlayerColor != pieceToCalc.PlayerOwner.PlayerColor
                            || (piece.Tier.TopOfTower
                                && piece.PlayerOwner.PlayerColor == pieceToCalc.PlayerOwner.PlayerColor
                                && AbilityToPiece.HasAbility(PreMoveAbility.CANNOT_BE_STACKED, piece.Piece.PieceType)))
                    )).Count();

                if (numPiecesBarringPath > 0)
                {
                    returnValue = true;
                    break;
                }

                evalLocation -= increment;
            }

            return returnValue;
        }

        #region Jump Exclusion
        private bool IsNotLShapedJump(Vector2 start, Vector2 destination)
        {
            Vector2 distance = new Vector2(
                start.x == destination.x ?
                    0 : Math.Abs(start.x - destination.x),
                start.y == destination.y ?
                    0 : Math.Abs(start.y - destination.y)
                );

            // Is Vertical, horizontal, or diagonal?
            return distance.x == 0
                || distance.y == 0
                || distance.x == distance.y;
        }

        /**
         * Requires Mre affected pieces along both paths of the jump
         * Paths of jump:
         * * File-first (change file, then rank)
         * * Rank-first (change rank, then file)
         * 
         * Note that all L-shaped jumps (Spy & Clandestinite only) are +/-1 file, +/- 2 ranks
         */
        private bool ShouldRemoveLShapedJumpDestination(PieceEV pieceToCalc, Vector2 destination, List<PieceEV> enemeyMreAffectedPieces)
        {
            List<Vector2> fileFirstPath = CalcFileFirstPath(pieceToCalc, destination);

            if (!DoesObstructingEnemyPieceExists(fileFirstPath, enemeyMreAffectedPieces))
            {
                return false;
            }

            List<Vector2> rankFirstPath = CalcRankFirstPath(pieceToCalc, destination);

            return DoesObstructingEnemyPieceExists(rankFirstPath, enemeyMreAffectedPieces);
        }

        private List<Vector2> CalcFileFirstPath(PieceEV pieceToCalc, Vector2 destination)
        {
            List<Vector2> returnValue = new List<Vector2>();
            float increment = (destination.y - pieceToCalc.Location.Location.y) / Math.Abs(destination.y - pieceToCalc.Location.Location.y);

            // No obstructing piece AT destination; already account for in other function
            // Thus no need to include destination location in returnedValue
            for (float rank = pieceToCalc.Location.Location.y; rank != destination.y; rank += increment)
            {
                returnValue.Add(new Vector2(destination.x, rank));
            }

            return returnValue;
        }

        private List<Vector2> CalcRankFirstPath(PieceEV pieceToCalc, Vector2 destination)
        {
            List<Vector2> returnValue = new List<Vector2>();
            float increment = (destination.y - pieceToCalc.Location.Location.y) / Math.Abs(destination.y - pieceToCalc.Location.Location.y);

            // No obstructing piece AT destination; already account for in other function
            // Thus no need to include destination location in returnedValue
            for (float rank = pieceToCalc.Location.Location.y; rank != destination.y; rank += increment)
            {
                returnValue.Add(new Vector2(pieceToCalc.Location.Location.x, rank));
            }

            return returnValue;
        }

        private bool DoesObstructingEnemyPieceExists(List<Vector2> lShapedJumpPath, List<PieceEV> enemeyMreAffectedPieces)
        {
            return enemeyMreAffectedPieces.Where(piece =>
                lShapedJumpPath.Contains(piece.Location.Location)).Count() > 0;
        }
        #endregion
        #endregion

        #region Exclude Check Violations
        private void ExcludeCheckViolations(PieceEV pieceEV, List<Vector2> returnValue, List<PieceEV> allPieces, IEntitiesDB entitiesDB)
        {
            PieceEV commander = allPieces.First(piece =>
                piece.Piece.PieceType == PieceType.COMMANDER && piece.PlayerOwner.PlayerColor == pieceEV.PlayerOwner.PlayerColor);
            List<PieceEV> commanderTowerPieces = FindPiecesAtLocation(commander.Location.Location, allPieces);

            // If Commander safely buried in a tower whose adjacent piece(s) are not changing, Commander cannot be in check this turn
            if (pieceEV.Location.Location != commander.Location.Location && IsCommanderBuried(commander, commanderTowerPieces))
            {
                return;
            }

            // If piece below Commander is enemy piece, moving another piece won't help
            if (pieceEV.ID.entityID != commander.ID.entityID && IsCommanderInDangerFromBelow(commander, commanderTowerPieces))
            {
                returnValue.RemoveAll(delegate (Vector2 v1)
                { return true; });

                return;
            }

            // Commander is topOfTower
            List<Vector2> destinationsToRemove = new List<Vector2>();

            foreach (Vector2 destination in returnValue)
            {
                // Commander in check cannot move to a tile occupied by a friendly piece
                if (pieceEV.ID.entityID == commander.ID.entityID
                    && DestinationOccupiedByFriendly(pieceEV.PlayerOwner.PlayerColor, destination, allPieces)
                    && IsCommanderInCheck(pieceEV.PlayerOwner.PlayerColor, entitiesDB))
                {
                    destinationsToRemove.Add(destination);
                    continue;
                }

                bool wasDestinationRemoved = ExcludeMovingCheckViolations(
                    pieceEV, commander, destination, allPieces, entitiesDB, false);

                if (IsMreStackPossible(pieceEV, destination, allPieces))
                {
                    wasDestinationRemoved &= ExcludeMovingCheckViolations(
                            pieceEV, commander, destination, allPieces, entitiesDB, true);
                }

                if (wasDestinationRemoved)
                {
                    destinationsToRemove.Add(destination);
                }
            }

            foreach (Vector2 removeDestination in destinationsToRemove)
            {
                returnValue.Remove(removeDestination);
            }
        }

        private bool ExcludeMovingCheckViolations(
            PieceEV pieceEV,
            PieceEV commander,
            Vector2 destination,
            List<PieceEV> allPieces,
            IEntitiesDB entitiesDB,
            bool stackMrePieceIfPossible)
        {
            // Make temp move while saving old info
            PreviousMoveState previousMoveState = SaveCurrentMove(pieceEV, destination, allPieces);
            PreviousTowerState? previousDestinationTowerState = previousMoveState.pieceCaptured.HasValue
                && BetrayalInEffect(previousMoveState.pieceToMove.Piece)
                ? SaveDestinationTowerState(previousMoveState, allPieces) : null;
            bool wasMrePieceStacked = MakeTemporaryMove(pieceEV, destination, allPieces, stackMrePieceIfPossible);

            if (!wasMrePieceStacked)
            {
                return false;
            }

            // If piece covers Commander, no threat to Commander
            if (pieceEV.ID.entityID != commander.ID.entityID
                && !commander.Tier.TopOfTower && pieceEV.Location.Location == commander.Location.Location)
            {
                RestorePreviousState(previousMoveState, previousDestinationTowerState);
                return false;
            }

            // If Commander captures enemy piece, it cannot put itself into check b/c of another enemy piece right below that piece
            if (pieceEV.ID.entityID == commander.ID.entityID && SecondFromTopTowerPiecesEnemy(pieceEV, allPieces))
            {
                RestorePreviousState(previousMoveState, previousDestinationTowerState);
                return true;
            }

            // Always re-calculate in case a captured MRE piece changes things
            List<PieceEV> enemyThreats = FindEnemyThreats(commander, pieceEV, allPieces, entitiesDB);

            // If no threats
            if (enemyThreats.Count == 0)
            {
                RestorePreviousState(previousMoveState, previousDestinationTowerState);
                return false;
            }

            List<PieceEV> actualThreats = FindActualEnemyThreats(commander, enemyThreats, allPieces, entitiesDB);

            if (actualThreats.Count > 0)
            {
                // Special scenario: Capture enemy lance to initiate Forced Rearrangement, then drop Catapult/Fortress to resolve check
                if (HaveCapturedForcedRearrangementPiece(previousMoveState)
                    && ForcedRearrangementCanResolveThreats(commander, previousMoveState.pieceCaptured.Value.Piece, actualThreats, allPieces, entitiesDB))
                {
                    // TODO Clean up boolean statement logic here
                    RestorePreviousState(previousMoveState, previousDestinationTowerState);
                    return false;
                }
                else
                {
                    RestorePreviousState(previousMoveState, previousDestinationTowerState);
                    return true;
                }
            }

            // Restore old info into values
            RestorePreviousState(previousMoveState, previousDestinationTowerState);
            return false;
        }

        // TODO Move to utility service/class and call from there
        private bool IsAdjacentPieceEnemy(PieceEV pieceToCompare, List<PieceEV> towerPieces)
        {
            switch(pieceToCompare.Tier.Tier)
            {
                case 1:
                case 3:
                    return pieceToCompare.PlayerOwner.PlayerColor != towerPieces[1].PlayerOwner.PlayerColor;
                case 2:
                    return pieceToCompare.PlayerOwner.PlayerColor != towerPieces[0].PlayerOwner.PlayerColor
                        || (towerPieces.Count > 2 && pieceToCompare.PlayerOwner.PlayerColor != towerPieces[2].PlayerOwner.PlayerColor);
                default:
                    throw new InvalidOperationException("Invalid tier number");
            }
        }

        private bool SecondFromTopTowerPiecesEnemy(PieceEV commander, List<PieceEV> allPieces)
        {
            List<PieceEV> piecesAtLocation = allPieces.Where(piece =>
                piece.Location.Location == commander.Location.Location && piece.ID.entityID != commander.ID.entityID).ToList();
            piecesAtLocation.Sort(delegate (PieceEV p1, PieceEV p2)
            { return p1.Tier.Tier.CompareTo(p2.Tier.Tier); });

            // Temporarily-modified commander not necessarily at piecesAtLocation
            if (piecesAtLocation.Count >= 1
                && commander.Tier.TopOfTower
                && piecesAtLocation[piecesAtLocation.Count - 1].ID.entityID != commander.ID.entityID
                && piecesAtLocation[0].Location.Location == commander.Location.Location)
            {
                piecesAtLocation.Add(commander);
            }

            return piecesAtLocation.Count >= 2
                && piecesAtLocation[piecesAtLocation.Count - 2].PlayerOwner.PlayerColor != commander.PlayerOwner.PlayerColor
                && CanImmobileCapture(piecesAtLocation[piecesAtLocation.Count - 2]); // No worries about enemy Fortresses, since they cannot immobile capture
        }

        private bool CanImmobileCapture(PieceEV piece)
        {
            return !AbilityToPiece.HasAbility(PreMoveAbility.CANNOT_IMMOBILE_CAPTURE, piece.Piece.PieceType);
        }

        private bool DestinationOccupiedByFriendly(PlayerColor playerColor, Vector2 destination, List<PieceEV> allPieces)
        {
            List<PieceEV> piecesAtLocation = allPieces.Where(piece => piece.Location.Location == destination).ToList();

            return piecesAtLocation.Count > 0
                && piecesAtLocation[piecesAtLocation.Count - 1].PlayerOwner.PlayerColor == playerColor;
        }

        #region Find Enemy Threats
        private List<PieceEV> FindEnemyThreats(PieceEV commander, PieceEV pieceToMove, List<PieceEV> allPieces, IEntitiesDB entitiesDB)
        {
            List<PieceEV> enemyPieces = allPieces.Where(piece =>
                piece.PlayerOwner.PlayerColor != commander.PlayerOwner.PlayerColor).ToList();
            List<PieceEV> piecesAtCurrentLocation = allPieces.Where(piece => // Min size one
                piece.Location.Location == pieceToMove.Location.Location).ToList();

            List<PieceEV> returnValue = enemyPieces.Where(piece =>
                piece.Tier.TopOfTower
                && ( piece.Location.Location == commander.Location.Location
                || CalcUnobstructedDestinationTiles(piece, allPieces, entitiesDB).Contains(commander.Location.Location))
                )
            .ToList();

            // One more scenario: Moving the pieceToMove could expose the underlying enemy piece that could check the Commander
            // That piece is UNABLE check the commander if other pieces obstruct it
            if (piecesAtCurrentLocation.Count > 1)
            {
                PieceEV pieceToEvaluate = piecesAtCurrentLocation[piecesAtCurrentLocation.Count - 2];

                if (pieceToEvaluate.PlayerOwner.PlayerColor != commander.PlayerOwner.PlayerColor
                && CalcUnobstructedDestinationTiles(pieceToEvaluate, allPieces, entitiesDB).Contains(commander.Location.Location))
                {
                    returnValue.Add(pieceToEvaluate);
                }
            }

            return returnValue;
        }

        private List<Vector2> CalcUnobstructedDestinationTiles(PieceEV piece, List<PieceEV> allPieces, IEntitiesDB entitiesDB)
        {
            return CalcDestinations(piece, allPieces, entitiesDB, false, false);
        }
        #endregion

        #region Previous Move State
        private PreviousMoveState SaveCurrentMove(PieceEV pieceToMove, Vector2 destination, List<PieceEV> allPieces)
        {
            List<PieceEV> piecesAtCurrentLocation = FindPiecesAtLocation(pieceToMove.Location.Location, allPieces); // Min size one
            List<PieceEV> topPieceAtDestination = allPieces.Where(piece => // Size one or zero
                piece.Location.Location == destination && piece.Tier.TopOfTower).ToList();

            PreviousMoveState returnValue = new PreviousMoveState
            {
                pieceToMove = SaveSingleState(pieceToMove),
                pieceBelow = piecesAtCurrentLocation.Count == 1
                    ? null : (PreviousPieceState?)SaveSingleState(piecesAtCurrentLocation[pieceToMove.Tier.Tier - 2]),
                pieceCaptured = topPieceAtDestination.Count == 0
                    ? null : (PreviousPieceState?)SaveSingleState(topPieceAtDestination[0])
            };

            return returnValue;
        }

        private PreviousTowerState? SaveDestinationTowerState(PreviousMoveState previousMoveState, List<PieceEV> allPieces)
        {
            PreviousTowerState returnValue = new PreviousTowerState
            {
                Pieces = new List<PreviousPieceState>()
            };

            List<PieceEV> piecesAtDestination = FindPiecesAtLocation(previousMoveState.pieceCaptured.Value.Location, allPieces); // Min size one

            foreach (PieceEV piece in piecesAtDestination)
            {
                returnValue.Pieces.Add(SaveSingleState(piece));
            }

            return returnValue;
        }

        private PreviousPieceState SaveSingleState(PieceEV pieceToSave)
        {
            return new PreviousPieceState
            {
                Piece = pieceToSave,
                PlayerColor = pieceToSave.PlayerOwner.PlayerColor,
                PieceType = pieceToSave.Piece.PieceType,
                Location = new Vector2(pieceToSave.Location.Location.x, pieceToSave.Location.Location.y),
                Tier = pieceToSave.Tier.Tier,
                TopOfTower = pieceToSave.Tier.TopOfTower
            };
        }

        private bool MakeTemporaryMove(
            PieceEV pieceToMove, Vector2 destination, List<PieceEV> allPieces, bool stackMrePieceIfPossible)
        {
            List<PieceEV> piecesAtCurrentLocation = FindPiecesAtLocation(pieceToMove.Location.Location, allPieces); // Min size one
            List<PieceEV> topPieceAtDestination = allPieces.Where(piece => // Size one or zero
                piece.Location.Location == destination && piece.Tier.TopOfTower).ToList();
            bool cannotCaptureBecauseBetrayViolatesTwoFileMove =
                CannotCaptureBecauseBetrayViolatesTwoFileMove(pieceToMove, destination, allPieces);

            if (stackMrePieceIfPossible
                && !(topPieceAtDestination.Count > 0
                    && topPieceAtDestination.Last().PlayerOwner.PlayerColor != pieceToMove.PlayerOwner.PlayerColor
                    && topPieceAtDestination.Last().Tier.Tier < 3
                    && IsMrePiece(topPieceAtDestination.Last())))
            {
                return false;
            }

            pieceToMove.Location.Location = destination;

            if (piecesAtCurrentLocation.Count > 1)
            {
                piecesAtCurrentLocation[piecesAtCurrentLocation.Count - 2].Tier.TopOfTower = true;
            }

            if (topPieceAtDestination.Count > 0)
            {
                topPieceAtDestination[0].Tier.TopOfTower = false;

                if (topPieceAtDestination[0].PlayerOwner.PlayerColor == pieceToMove.PlayerOwner.PlayerColor
                    || cannotCaptureBecauseBetrayViolatesTwoFileMove
                    || stackMrePieceIfPossible)
                {
                    pieceToMove.Tier.Tier = topPieceAtDestination[0].Tier.Tier + 1;
                }
                else
                {
                    pieceToMove.Tier.Tier = topPieceAtDestination[0].Tier.Tier;
                    topPieceAtDestination[0].Location.Location = BoardConst.HAND_LOCATION;

                    if (BetrayalInEffect(pieceToMove))
                    {
                        List<PieceEV> piecesAtDestination = FindPiecesAtLocation(destination, allPieces); // Min size one
                        BetrayalEffectOnTower(piecesAtDestination, pieceToMove.PlayerOwner.PlayerColor);
                    }
                }
            }

            return true;
        }

        private void RestorePreviousState(PreviousMoveState previousState, PreviousTowerState? previousDestinationTowerState)
        {
            if (previousDestinationTowerState.HasValue)
            {
                foreach (PreviousPieceState previousTowerPieceState in previousDestinationTowerState.Value.Pieces)
                {
                    RestoreSingleState(previousTowerPieceState);
                }
            }

            RestoreSingleState(previousState.pieceToMove);

            if (previousState.pieceBelow.HasValue)
            {
                RestoreSingleState(previousState.pieceBelow.Value);
            }

            if (previousState.pieceCaptured.HasValue)
            {
                RestoreSingleState(previousState.pieceCaptured.Value);
            }
        }

        private void RestoreSingleState(PreviousPieceState previousState)
        {
            PieceEV pieceToRestore = previousState.Piece;
            pieceToRestore.PlayerOwner.PlayerColor = previousState.PlayerColor;
            pieceToRestore.Piece.PieceType = previousState.PieceType;
            pieceToRestore.Location.Location = previousState.Location;
            pieceToRestore.Tier.Tier = previousState.Tier;
            pieceToRestore.Tier.TopOfTower = previousState.TopOfTower;
        }
        #endregion

        #region Forced Rearrangement
        private bool DoesDropResolveCheck(
            PieceEV commander,
            PieceEV pieceToDrop,
            Vector2 location,
            List<PieceEV> piecesAtLocation,
            List<PieceEV> actualThreats,
            List<PieceEV> allPieces,
            IEntitiesDB entitiesDB)
        {
            bool returnValue;
            PlayerColor originalPlayerColor = pieceToDrop.PlayerOwner.PlayerColor;
            PieceEV? topPieceAtDestination = null;

            if (piecesAtLocation.Count > 0)
            {
                topPieceAtDestination = piecesAtLocation[piecesAtLocation.Count - 1];
                topPieceAtDestination.Value.Tier.TopOfTower = false;
            }

            pieceToDrop.PlayerOwner.PlayerColor = commander.PlayerOwner.PlayerColor;
            pieceToDrop.Tier.TopOfTower = true;
            pieceToDrop.Tier.Tier = topPieceAtDestination.HasValue ? topPieceAtDestination.Value.Tier.Tier + 1 : 1;

            List<PieceEV> newActualThreats = FindActualEnemyThreats(commander, actualThreats, allPieces, entitiesDB);
            returnValue = newActualThreats.Count == 0;

            pieceToDrop.PlayerOwner.PlayerColor = originalPlayerColor;
            pieceToDrop.Tier.TopOfTower = false;
            pieceToDrop.Tier.Tier = 0;
            pieceToDrop.Location.Location = BoardConst.HAND_LOCATION;

            if (topPieceAtDestination.HasValue)
            {
                topPieceAtDestination.Value.Tier.TopOfTower = true;
            }

            return returnValue;
        }

        private bool HaveCapturedForcedRearrangementPiece(PreviousMoveState previousMoveState)
        {
            return previousMoveState.pieceCaptured.HasValue
                && AbilityToPiece.HasAbility(PostMoveAbility.FORCED_REARRANGEMENT, previousMoveState.pieceCaptured.Value.Piece.Piece.PieceType);
        }

        private bool IsValidDrop(List<PieceEV> piecesAtLocation, IEntitiesDB entitiesDB)
        {
            return preDropService.IsValidForcedRearrangementDrop(piecesAtLocation, entitiesDB);
        }
        #endregion
        #endregion

        #region Mobile Range Expansion
        private int CalcTierToUse(PieceEV piece, List<PieceEV> allPieces)
        {
            return CanMobileRangeExpansion(piece)
                && (IsAffectedByMobileRangeExpansionRadial(piece.PlayerOwner.PlayerColor, piece.Location.Location, allPieces)
                || IsAffectedByMobileRangeExpansionLine(piece.PlayerOwner.PlayerColor, piece.Location.Location, allPieces))
                ? Math.Min(piece.Tier.Tier + 1, 3) : piece.Tier.Tier;
        }

        private bool CanMobileRangeExpansion(PieceEV piece)
        {
            return !AbilityToPiece.HasAbility(PreMoveAbility.CANNOT_MOBILE_RANGE_EXPANSION, piece.Piece.PieceType);
        }

        private bool IsMrePiece(PieceEV pieceEV)
        {
            return AbilityToPiece.HasAbility(PreMoveAbility.MOBILE_RANGE_EXPANSION_RADIAL, pieceEV.Piece.PieceType)
                || AbilityToPiece.HasAbility(PreMoveAbility.MOBILE_RANGE_EXPANSION_LINE, pieceEV.Piece.PieceType);
        }

        private bool IsAffectedByMobileRangeExpansionRadial(
            PlayerColor playerColor, Vector2 location, List<PieceEV> allPieces, List<PieceEV> piecesMreRadial = null)
        {
            if (piecesMreRadial == null)
            {
                piecesMreRadial = FindPiecesMreRadial(playerColor, allPieces);
            }

            if (piecesMreRadial.Count == 0)
            {
                return false;
            }

            List<PieceEV> piecesMreRadialInRange = piecesMreRadial.Where(piece =>
                distanceService.CalcAbsoluteDistance(location, piece.Location.Location) <= 2).ToList();

            return piecesMreRadialInRange.Count > 0;
        }

        private bool IsAffectedByMobileRangeExpansionLine(
            PlayerColor playerColor, Vector2 location, List<PieceEV> allPieces, List<PieceEV> piecesMreLine = null)
        {
            if (piecesMreLine == null)
            {
                piecesMreLine = FindPiecesMreLine(playerColor, allPieces);
            }

            if (piecesMreLine.Count == 0)
            {
                return false;
            }

            List<PieceEV> piecesMreLineInRange = piecesMreLine.Where(piece =>
                distanceService.IsAhead(piece.Location.Location, location, playerColor)).ToList();

            return piecesMreLineInRange.Count > 0;
        }

        private List<PieceEV> FindPiecesMreRadial(PlayerColor playerColor, List<PieceEV> allPieces)
        {
            return allPieces.Where(piece =>
                   piece.PlayerOwner.PlayerColor == playerColor
                   && AbilityToPiece.HasAbility(PreMoveAbility.MOBILE_RANGE_EXPANSION_RADIAL, piece.Piece.PieceType)).ToList();
        }

        private List<PieceEV> FindPiecesMreLine(PlayerColor playerColor, List<PieceEV> allPieces)
        {
            return allPieces.Where(piece =>
                       piece.PlayerOwner.PlayerColor == playerColor
                       && AbilityToPiece.HasAbility(PreMoveAbility.MOBILE_RANGE_EXPANSION_LINE, piece.Piece.PieceType)).ToList();
        }

        private List<PieceEV> FindMobileRangeExpansionAffectedPieces(PlayerColor playerColor, List<PieceEV> allPieces)
        {
            List<PieceEV> returnValue = FindMreRadialAffectedPieces(playerColor, allPieces);
            returnValue.AddRange(FindMreLineAffectedPieces(playerColor, allPieces));

            return returnValue;
        }

        private List<PieceEV> FindMreRadialAffectedPieces(PlayerColor playerColor, List<PieceEV> allPieces)
        {
            // Mobile Range Expansion pieces affect themselves
            List<PieceEV> mreRadialPieces = FindPiecesMreRadial(playerColor, allPieces);

            return allPieces.Where(piece =>
                piece.PlayerOwner.PlayerColor == playerColor
                && CanMobileRangeExpansion(piece)
                && IsAffectedByMobileRangeExpansionRadial(playerColor, piece.Location.Location, allPieces, mreRadialPieces)).ToList();
        }

        private List<PieceEV> FindMreLineAffectedPieces(PlayerColor playerColor, List<PieceEV> allPieces)
        {
            // Mobile Range Expansion pieces affect themselves
            List<PieceEV> mreLinePieces = FindPiecesMreLine(playerColor, allPieces);

            return allPieces.Where(piece =>
                piece.PlayerOwner.PlayerColor == playerColor
                && CanMobileRangeExpansion(piece)
                && IsAffectedByMobileRangeExpansionLine(playerColor, piece.Location.Location, allPieces, mreLinePieces)).ToList();
        }

        private bool IsMreStackPossible(PieceEV pieceToMove, Vector2 destination, List<PieceEV> allPieces)
        {
            List<PieceEV> piecesAtCurrentLocation = FindPiecesAtLocation(pieceToMove.Location.Location, allPieces); // Min size one
            List<PieceEV> topPieceAtDestination = allPieces.Where(piece => // Size one or zero
                piece.Location.Location == destination && piece.Tier.TopOfTower).ToList();

            return topPieceAtDestination.Count > 0
                    && topPieceAtDestination.Last().PlayerOwner.PlayerColor != pieceToMove.PlayerOwner.PlayerColor
                    && topPieceAtDestination.Last().Tier.Tier < 3
                    && IsMrePiece(topPieceAtDestination.Last());
        }
        #endregion

        #region Betrayal
        private bool BetrayalInEffect(PieceEV pieceToMove)
        {
            return pieceToMove.Tier.TopOfTower
                && AbilityToPiece.HasAbility(PostMoveAbility.BETRAYAL, pieceToMove.Piece.PieceType);
        }

        private void BetrayalEffectOnTower(List<PieceEV> towerPieces, PlayerColor betrayalColor)
        {
            foreach (PieceEV piece in towerPieces)
            {
                if (piece.PlayerOwner.PlayerColor != betrayalColor)
                {
                    piece.PlayerOwner.PlayerColor = betrayalColor;
                    piece.Piece.PieceType = piece.Piece.PieceType == piece.Piece.Front ? piece.Piece.Back : piece.Piece.Front;
                }
            }
        }

        private bool CannotCaptureBecauseBetrayViolatesTwoFileMove(PieceEV pieceToMove, Vector2 destination, List<PieceEV> allPieces)
        {
            // Only care about Bronze pieces, which only have Single move sets
            if (!AbilityToPiece.HasAbility(PostMoveAbility.BETRAYAL, pieceToMove.Piece.PieceType)
                || !AbilityToPiece.HasAbility(PreMoveAbility.TWO_FILE_MOVE, pieceToMove.Piece.PieceType))
            {
                return false;
            }

            bool returnValue = false;
            List<PieceEV> towerPieces = FindPiecesAtLocation(destination, allPieces);

            // Do not examine top piece, since it could be captured
            for (int i = 0; i < towerPieces.Count - 1; ++i)
            {
                if (towerPieces[i].Piece.PieceType == pieceToMove.Piece.Front)
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }
        #endregion

        #region Utility
        private List<PieceEV> FindPiecesAtLocation(Vector2 location, List<PieceEV> allPieces)
        {
            List<PieceEV> returnValue = allPieces.Where(piece => piece.Location.Location == location).ToList();
            returnValue.Sort(delegate (PieceEV p1, PieceEV p2)
            { return p1.Tier.Tier.CompareTo(p2.Tier.Tier); });

            return returnValue;
        }
        #endregion
    }
}
