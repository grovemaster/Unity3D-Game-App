using Data.Enum.Player;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Board
{
    public static class DestinationTileService
    {
        /**
         * This will return values exceeding board boundaries (such as below the zero-th rank).  It is the
         * responsibility of the client to not abuse this data.
         */
        public static List<Vector3> CalcDestinationTileLocations(
            PieceEV pieceEV, IEntitiesDB entitiesDB, List<PieceEV> allPieces = null)
        {
            List<Vector3> returnValue = new List<Vector3>();

            if (allPieces == null)
            {
                // Current piece included, doesn't matter for current calc
                allPieces = PieceService.FindAllBoardPieces(entitiesDB).ToList();
            }

            returnValue.AddRange(CalcSingleDestinations(pieceEV, allPieces));

            return returnValue;
        }

        public static HashSet<Vector3> CalcDestinationTileLocations(PieceEV[] pieces, IEntitiesDB entitiesDB)
        {
            HashSet<Vector3> returnValue = new HashSet<Vector3>();
            List<PieceEV> allPieces = PieceService.FindAllBoardPieces(entitiesDB).ToList();

            for (int i = 0; i < pieces.Length; ++i)
            {
                returnValue.UnionWith(CalcDestinationTileLocations(pieces[i], entitiesDB, allPieces));
            }

            return returnValue;
        }

        private static List<Vector3> CalcSingleDestinations(PieceEV pieceEV, List<PieceEV> allPieces)
        {
            List<Vector3> returnValue = GetRawSingleDestinationLocations(pieceEV);
            AdjustRawDataWithPieceLocationAndDirection(pieceEV, returnValue);
            ExcludeDestinationsWithFriendlyTier3Tower(pieceEV, returnValue, allPieces);
            // Do NOT allow destinations other pieces in the way
            ExcludeDestinationsWithObstructingPieces(pieceEV, returnValue, allPieces);

            return returnValue;
        }

        private static List<Vector3> GetRawSingleDestinationLocations(PieceEV pieceEV)
        {
            return PieceService.CreateIPieceData(pieceEV.piece.PieceType).Tiers()[pieceEV.tier.Tier - 1].Single()
                .Select(x => new Vector3(x.x, x.y, 0)).ToList(); // Change z-value from >=1 to 0
        }

        private static void AdjustRawDataWithPieceLocationAndDirection(
            PieceEV pieceEV, List<Vector3> rawLocationData)
        {
            // Add piece's location to value
            for (int i = 0; i < rawLocationData.Count; ++i)
            {
                rawLocationData[i] = new Vector3(
                    pieceEV.location.Location.x + (rawLocationData[i].x * (int)pieceEV.piece.Direction),
                    pieceEV.location.Location.y + (rawLocationData[i].y * (int)pieceEV.piece.Direction),
                    rawLocationData[i].z);
            }
        }

        /**
         * If destination contains a tier 3 tower with a friendly piece on top,
         * then that tile is not a valid destination tile.
         */
        private static void ExcludeDestinationsWithFriendlyTier3Tower(
            PieceEV pieceToCalc, List<Vector3> destinations, List<PieceEV> allPieces)
        {
            List<Vector3> destinationsToRemove = new List<Vector3>();

            foreach (Vector3 destination in destinations)
            {
                if (HasFriendlyTier3Tower(pieceToCalc, destination, allPieces))
                {
                    destinationsToRemove.Add(destination);
                }
            }

            foreach (Vector3 removeDestination in destinationsToRemove)
            {
                destinations.Remove(removeDestination);
            }
        }

        private static bool HasFriendlyTier3Tower(
            PieceEV pieceToCalc, Vector2 destination, List<PieceEV> allPieces)
        {
            PlayerColor friendlyColor = pieceToCalc.playerOwner.PlayerColor;

            int numPiecesBarringPath = allPieces.Where(piece =>
                piece.tier.Tier == 3
                && piece.playerOwner.PlayerColor == friendlyColor
                && destination.x == piece.location.Location.x
                && destination.y == piece.location.Location.y).Count();

            return numPiecesBarringPath > 0;
        }

        /**
         * Potentially modify param destinations, by removing locations with piece(s) in the way
         * 
         * Some destinations are in different rank AND file as pieceToCalc's current location
         */
        private static void ExcludeDestinationsWithObstructingPieces(
            PieceEV pieceToCalc, List<Vector3> destinations, List<PieceEV> allPieces)
        {
            List<Vector3> destinationsToRemove = new List<Vector3>();
            /*
             * Types of destinations
             * * One tile away, horizontally, vertically, or diagonally
             * More than one tile away,
             * * Same rank, different file
             * * Same file, different rank
             * * Different rank, different file
             */

            // for loop hopLocations, since that count will often be less than allPiece's count
            foreach (Vector3 destination in destinations)
            {
                if (ShouldRemoveDestination(pieceToCalc, destination, allPieces))
                {
                    destinationsToRemove.Add(destination);
                }
            }

            foreach (Vector3 removeDestination in destinationsToRemove)
            {
                destinations.Remove(removeDestination);
            }
        }

        private static bool ShouldRemoveDestination(
            PieceEV pieceToCalc, Vector2 destination, List<PieceEV> allPieces)
        {
            bool returnValue = false;
            Vector2 pieceLocation = pieceToCalc.location.Location;

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
                    evalLocation.x == piece.location.Location.x
                    && evalLocation.y == piece.location.Location.y).Count();

                if (numPiecesBarringPath > 0)
                {
                    returnValue = true;
                    break;
                }

                evalLocation -= increment;
            }

            return returnValue;
        }
    }
}
