using Data.Enum;
using Data.Piece;
using Data.Piece.Back.Gold;
using Data.Piece.Front.Pawn;
using System;

namespace Service.Piece.Factory
{
    public class PieceFactory
    {
        public IPieceData CreateIPieceData(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.PAWN:
                    return new PawnData();
                case PieceType.GOLD:
                    return new GoldData();
                default:
                    throw new InvalidOperationException("Invalid PieceType when creating IPieceData");
            }
        }
    }
}
