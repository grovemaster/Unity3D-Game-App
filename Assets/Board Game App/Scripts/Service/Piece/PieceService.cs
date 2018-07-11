using Data.Enum;
using Data.Piece;
using Data.Piece.Front.Pawn;
using ECS.EntityView.Piece;
using Svelto.ECS;
using System;

namespace Service.Piece
{
    public class PieceService
    {
        public static PieceEV FindPieceEV(int entityId, IEntitiesDB entitiesDB)
        {
            uint index;
            var entityViews = entitiesDB.QueryEntitiesAndIndex<PieceEV>(new EGID(entityId), out index);

            return entityViews[index];
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
    }
}
