using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;
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
        public static List<Vector3> CalcDestinationTileLocations(int pieceEntityId, IEntitiesDB entitiesDB)
        {
            PieceEV pieceEV = PieceService.FindPieceEV(pieceEntityId, entitiesDB);

            List<Vector3> returnValue = GetRawDestinationLocations(pieceEV);
            AdjustRawDataWithPieceLocationAndDirection(pieceEV, returnValue);

            return returnValue;
        }

        public static HashSet<Vector3> CalcDestinationTileLocations(PieceEV[] pieces, IEntitiesDB entitiesDB)
        {
            HashSet<Vector3> returnValue = new HashSet<Vector3>();

            for (int i = 0; i < pieces.Length; ++i)
            {
                returnValue.UnionWith(CalcDestinationTileLocations(pieces[i].ID.entityID, entitiesDB));
            }

            return returnValue;
        }

        private static List<Vector3> GetRawDestinationLocations(PieceEV pieceEV)
        {
            return PieceService.CreateIPieceData(pieceEV.piece.PieceType).Tiers()[0].Single()
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
    }
}
