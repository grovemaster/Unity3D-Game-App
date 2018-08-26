using Data.Enum.Piece;
using Data.Piece;
using Data.Piece.Back.Arrow;
using Data.Piece.Back.Bronze;
using Data.Piece.Back.Gold;
using Data.Piece.Front.Bow;
using Data.Piece.Front.Commander;
using Data.Piece.Front.Pawn;
using Data.Piece.Front.Spy;
using System;

namespace Service.Piece.Factory
{
    public class PieceFactory
    {
        public IPieceData CreateIPieceData(PieceType pieceType)
        {
            switch (pieceType)
            {
                // Front
                case PieceType.COMMANDER:
                    return new CommanderData();
                case PieceType.PAWN:
                    return new PawnData();
                case PieceType.SPY:
                    return new SpyData();
                case PieceType.BOW:
                    return new BowData();

                // Back
                case PieceType.ARROW:
                    return new ArrowData();
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
