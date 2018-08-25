using Data.Enum.Piece;
using Data.Piece;
using Data.Piece.Back.Bronze;
using Data.Piece.Back.Gold;
using Data.Piece.Front.Commander;
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
                case PieceType.COMMANDER:
                    return new CommanderData();
                case PieceType.PAWN:
                    return new PawnData();
                case PieceType.BRONZE:
                    return new BronzeData();
                case PieceType.GOLD:
                    return new GoldData();
                default:
                    throw new InvalidOperationException("Invalid PieceType when creating IPieceData");
            }
        }
    }
}
