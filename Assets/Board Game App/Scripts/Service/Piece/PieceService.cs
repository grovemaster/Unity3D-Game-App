using Data.Enum;
using Data.Piece;
using Data.Piece.Front.Pawn;
using ECS.EntityView.Piece;
using Service.Common;
using Svelto.ECS;
using System;

namespace Service.Piece
{
    public class PieceService
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
    }
}
