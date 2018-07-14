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
        public static List<Vector3> FindDestinationTileLocations(int pieceEntityId, IEntitiesDB entitiesDB)
        {
            PieceEV pieceEV = PieceService.FindPieceEV(pieceEntityId, entitiesDB);
            
            List<Vector3> returnValue = GetRawDestinationLocations(pieceEV);
            AdjustRawDataWithPieceLocation(pieceEV, returnValue);

            return returnValue;
        }

        private static List<Vector3> GetRawDestinationLocations(PieceEV pieceEV)
        {
            return PieceService.CreateIPieceData(pieceEV.piece.PieceType).Tiers()[0].Single()
                .Select(x => new Vector3(x.x, x.y, 0)).ToList(); // Change z-value from >=1 to 0
        }

        private static void AdjustRawDataWithPieceLocation(PieceEV pieceEV, List<Vector3> rawLocationData)
        {
            // Add piece's location to value
            for (int i = 0; i < rawLocationData.Count; ++i)
            {
                rawLocationData[i] = new Vector3(
                    rawLocationData[i].x + pieceEV.location.Location.x,
                    rawLocationData[i].y + pieceEV.location.Location.y,
                    rawLocationData[i].z);
            }
        }
    }
}
