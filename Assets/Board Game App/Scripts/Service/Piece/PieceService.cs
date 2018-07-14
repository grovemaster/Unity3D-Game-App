using Data.Enum;
using Data.Piece;
using Data.Piece.Front.Pawn;
using ECS.EntityView.Piece;
using Service.Common;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Service.Piece
{
    public static class PieceService
    {
        public static PieceEV FindPieceEV(int entityId, IEntitiesDB entitiesDB)
        {
            return CommonService.FindEntity<PieceEV>(entityId, entitiesDB);
        }

        public static IPieceData CreateIPieceData(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.PAWN:
                    return new PawnData();
                default:
                    throw new InvalidOperationException("Invalid PieceType when creating IPieceData");
            }
        }

        public static PieceEV[] FindAllPieceEVs(IEntitiesDB entitiesDB, out int count)
        {
            return CommonService.FindAllEntities<PieceEV>(entitiesDB, out count);
        }

        public static PieceEV? FindPieceEVById(int? entityId, IEntitiesDB entitiesDB)
        {
            PieceEV returnValue = CommonService.FindEntityById<PieceEV>(entityId, entitiesDB);

            return returnValue.ID.entityID != 0 ? (PieceEV?)returnValue : null;
        }

        // TODO This will become a list once towers are enabled
        public static PieceEV? FindPieceByLocation(Vector3 location, IEntitiesDB entitiesDB)
        {
            PieceEV? returnValue = null;

            int count;
            PieceEV[] pieces = FindAllPieceEVs(entitiesDB, out count);

            for (int i = 0; i < count; ++i)
            {
                // Tile always on z=0, pieces always on z>=1
                if (pieces[i].location.Location.x == location.x
                    && pieces[i].location.Location.y == location.y)
                {
                    returnValue = pieces[i];
                    break;
                }
            }

            return returnValue;
        }
    }
}
