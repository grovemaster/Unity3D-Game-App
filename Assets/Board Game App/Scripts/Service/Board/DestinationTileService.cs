using Data.Check.PreviousMove;
using Data.Constants.Board;
using Data.Enum.Piece;
using Data.Enum.Player;
using ECS.EntityView.Piece;
using Service.Piece.Factory;
using Service.Piece.Find;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Board
{
    public class DestinationTileService
    {
        private PieceFactory pieceFactory = new PieceFactory();
        private PieceFindService pieceFindService = new PieceFindService();

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

            returnValue.AddRange(CalcSingleDestinations(pieceEV, allPieces, entitiesDB, excludeCheckViolations));

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
        #endregion

        private List<Vector2> CalcSingleDestinations(
            PieceEV pieceEV,
            List<PieceEV> allPieces,
            IEntitiesDB entitiesDB,
            bool excludeCheckViolations = false,
            bool excludeObstructedDestinations = true)
        {
            bool useGoldMovement = IsOpponentPieceDirectlyBelow(pieceEV, allPieces);
            List<Vector2> returnValue = GetRawSingleDestinationLocations(pieceEV, useGoldMovement);
            AdjustRawDataWithPieceLocationAndDirection(pieceEV, returnValue);
            ExcludeOutOfBoard(returnValue);
            ExcludeDestinationsWithFriendlyTier3Tower(pieceEV, returnValue, allPieces);

            if (excludeObstructedDestinations) // Do NOT allow destinations other pieces in the way
            {
                ExcludeDestinationsWithObstructingPieces(pieceEV, returnValue, allPieces);
            }

            if (excludeCheckViolations) // Should only happen for turn player
            {
                ExcludeCheckViolations(pieceEV, returnValue, allPieces, entitiesDB);
            }

            return returnValue;
        }

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

        private List<Vector2> GetRawSingleDestinationLocations(PieceEV pieceEV, bool useGoldMovement)
        {
            PieceType pieceToCreate = !useGoldMovement ? pieceEV.Piece.PieceType : PieceType.GOLD;

            return pieceFactory.CreateIPieceData(pieceToCreate).Tiers[pieceEV.Tier.Tier - 1].Single;
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

        /**
         * Potentially modify param destinations, by removing locations with piece(s) in the way
         * 
         * Some destinations are in different rank AND file as pieceToCalc's current location
         */
        private void ExcludeDestinationsWithObstructingPieces(
            PieceEV pieceToCalc, List<Vector2> destinations, List<PieceEV> allPieces)
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
                if (ShouldRemoveDestination(pieceToCalc, destination, allPieces))
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
            PieceEV pieceToCalc, Vector2 destination, List<PieceEV> allPieces)
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
                    evalLocation == piece.Location.Location).Count();

                if (numPiecesBarringPath > 0)
                {
                    returnValue = true;
                    break;
                }

                evalLocation -= increment;
            }

            return returnValue;
        }
        #endregion

        #region Exclude Check Violations
        private void ExcludeCheckViolations(PieceEV pieceEV, List<Vector2> returnValue, List<PieceEV> allPieces, IEntitiesDB entitiesDB)
        {
            PieceEV commander = allPieces.First(piece =>
                piece.Piece.PieceType == PieceType.COMMANDER && piece.PlayerOwner.PlayerColor == pieceEV.PlayerOwner.PlayerColor);
            List<PieceEV> commanderTowerPieces = allPieces.Where(piece => piece.Location.Location == commander.Location.Location).ToList();
            commanderTowerPieces.Sort(delegate (PieceEV p1, PieceEV p2)
                { return p1.Tier.Tier.CompareTo(p2.Tier.Tier); });

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
            List<PieceEV> enemyThreats = new List<PieceEV>();

            if (pieceEV.ID.entityID != commander.ID.entityID)
            {
                enemyThreats = FindEnemyThreats(commander, pieceEV, allPieces, entitiesDB);

                if (enemyThreats.Count == 0)
                {
                    return;
                }
            }

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

                // Make temp move while saving old info
                PreviousMoveState previousMoveState = SaveCurrentMove(pieceEV, destination, allPieces);
                MakeTemporaryMove(pieceEV, destination, allPieces);

                // If piece covers Commander, no threat to Commander
                if (pieceEV.ID.entityID != commander.ID.entityID
                    && !commander.Tier.TopOfTower && pieceEV.Location.Location == commander.Location.Location)
                {
                    RestorePreviousState(previousMoveState);
                    continue;
                }

                // If Commander captures enemy piece, it cannot put itself into check b/c of another enemy piece right below that piece
                if (pieceEV.ID.entityID == commander.ID.entityID && SecondFromTopTowerPiecesEnemy(pieceEV, allPieces))
                {
                    destinationsToRemove.Add(destination);
                    RestorePreviousState(previousMoveState);
                    continue;
                }

                if (pieceEV.ID.entityID == commander.ID.entityID)
                {
                    enemyThreats = FindEnemyThreats(commander, pieceEV, allPieces, entitiesDB);
                }

                // If no threats
                if (enemyThreats.Count == 0)
                {
                    RestorePreviousState(previousMoveState);
                    continue;
                }

                foreach (PieceEV threat in enemyThreats)
                {
                    if (threat.Tier.TopOfTower // Temporarily moved piece may have captured/stacked this enemy piece
                        && (threat.Location.Location == commander.Location.Location
                        || CalcSingleDestinations(threat, allPieces, entitiesDB, false).Contains(commander.Location.Location)))
                    {
                        destinationsToRemove.Add(destination);
                        break;
                    }
                }

                // Restore old info into values
                RestorePreviousState(previousMoveState);
            }

            foreach (Vector2 removeDestination in destinationsToRemove)
            {
                returnValue.Remove(removeDestination);
            }
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

            // Temporarily-modified commander not necessarily at piecesAtLocation
            if (piecesAtLocation.Count >= 1
                && commander.Tier.TopOfTower
                && piecesAtLocation[piecesAtLocation.Count - 1].ID.entityID != commander.ID.entityID
                && piecesAtLocation[0].Location.Location == commander.Location.Location)
            {
                piecesAtLocation.Add(commander);
            }

            return piecesAtLocation.Count >= 2
                && piecesAtLocation[piecesAtLocation.Count - 2].PlayerOwner.PlayerColor != commander.PlayerOwner.PlayerColor;
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
            return CalcSingleDestinations(piece, allPieces, entitiesDB, false, false);
        }
        #endregion

        #region Previous Move State
        private PreviousMoveState SaveCurrentMove(PieceEV pieceToMove, Vector2 destination, List<PieceEV> allPieces)
        {
            List<PieceEV> piecesAtCurrentLocation = allPieces.Where(piece => // Min size one
                piece.Location.Location == pieceToMove.Location.Location).ToList();
            piecesAtCurrentLocation.Sort(delegate (PieceEV p1, PieceEV p2)
            { return p1.Tier.Tier.CompareTo(p2.Tier.Tier); });
            List<PieceEV> topPieceAtDestination = allPieces.Where(piece => // Size one or zero
                piece.Location.Location == destination && piece.Tier.TopOfTower).ToList();

            PreviousMoveState returnValue = new PreviousMoveState
            {
                pieceToMove = new PreviousPieceState
                {
                    Piece = pieceToMove,
                    Location = new Vector2(pieceToMove.Location.Location.x, pieceToMove.Location.Location.y),
                    Tier = pieceToMove.Tier.Tier,
                    TopOfTower = pieceToMove.Tier.TopOfTower
                },

                pieceBelow = piecesAtCurrentLocation.Count == 1 ? null : (PreviousPieceState?)new PreviousPieceState
                {
                    Piece = piecesAtCurrentLocation[pieceToMove.Tier.Tier - 2],
                    Location = new Vector2(
                        piecesAtCurrentLocation[pieceToMove.Tier.Tier - 2].Location.Location.x,
                        piecesAtCurrentLocation[pieceToMove.Tier.Tier - 2].Location.Location.y),
                    Tier = piecesAtCurrentLocation[pieceToMove.Tier.Tier - 2].Tier.Tier,
                    TopOfTower = piecesAtCurrentLocation[pieceToMove.Tier.Tier - 2].Tier.TopOfTower
                },

                pieceCaptured = topPieceAtDestination.Count == 0 ? null : (PreviousPieceState?)new PreviousPieceState
                    {
                        Piece = topPieceAtDestination[0],
                        Location = new Vector2(topPieceAtDestination[0].Location.Location.x, topPieceAtDestination[0].Location.Location.y),
                        Tier = topPieceAtDestination[0].Tier.Tier,
                        TopOfTower = topPieceAtDestination[0].Tier.TopOfTower
                    }
            };

            return returnValue;
        }

        private void MakeTemporaryMove(PieceEV pieceToMove, Vector2 destination, List<PieceEV> allPieces)
        {
            List<PieceEV> piecesAtCurrentLocation = allPieces.Where(piece => // Min size one
                piece.Location.Location == pieceToMove.Location.Location).ToList();
            piecesAtCurrentLocation.Sort(delegate (PieceEV p1, PieceEV p2)
            { return p1.Tier.Tier.CompareTo(p2.Tier.Tier); });
            List<PieceEV> topPieceAtDestination = allPieces.Where(piece => // Size one or zero
                piece.Location.Location == destination && piece.Tier.TopOfTower).ToList();

            pieceToMove.Location.Location = destination;

            if (piecesAtCurrentLocation.Count > 1)
            {
                piecesAtCurrentLocation[piecesAtCurrentLocation.Count - 2].Tier.TopOfTower = true;
            }

            if (topPieceAtDestination.Count > 0)
            {
                topPieceAtDestination[0].Tier.TopOfTower = false;

                if (topPieceAtDestination[0].PlayerOwner.PlayerColor == pieceToMove.PlayerOwner.PlayerColor)
                {
                    pieceToMove.Tier.Tier = topPieceAtDestination[0].Tier.Tier + 1;
                }
                else
                {
                    pieceToMove.Tier.Tier = topPieceAtDestination[0].Tier.Tier;
                    topPieceAtDestination[0].Location.Location = new Vector2(-1, -1);
                }
            }
        }

        private void RestorePreviousState(PreviousMoveState previousState)
        {
            PieceEV pieceMoved = previousState.pieceToMove.Piece;
            pieceMoved.Location.Location = previousState.pieceToMove.Location;
            pieceMoved.Tier.Tier = previousState.pieceToMove.Tier;
            pieceMoved.Tier.TopOfTower = previousState.pieceToMove.TopOfTower;

            if (previousState.pieceBelow.HasValue)
            {
                PieceEV pieceBelow = previousState.pieceBelow.Value.Piece;
                pieceBelow.Location.Location = previousState.pieceBelow.Value.Location;
                pieceBelow.Tier.Tier = previousState.pieceBelow.Value.Tier;
                pieceBelow.Tier.TopOfTower = previousState.pieceBelow.Value.TopOfTower;
            }

            if (previousState.pieceCaptured.HasValue)
            {
                PieceEV pieceCaptured = previousState.pieceCaptured.Value.Piece;
                pieceCaptured.Location.Location = previousState.pieceCaptured.Value.Location;
                pieceCaptured.Tier.Tier = previousState.pieceCaptured.Value.Tier;
                pieceCaptured.Tier.TopOfTower = previousState.pieceCaptured.Value.TopOfTower;
            }
        }
        #endregion
        #endregion
    }
}
