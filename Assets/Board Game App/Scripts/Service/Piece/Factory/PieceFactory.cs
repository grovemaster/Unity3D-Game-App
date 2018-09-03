using Data.Enums.Piece;
using Data.Piece;
using Data.Piece.Back.Arrow;
using Data.Piece.Back.Bronze;
using Data.Piece.Back.Clandestinite;
using Data.Piece.Back.DragonKing;
using Data.Piece.Back.Gold;
using Data.Piece.Back.Lance;
using Data.Piece.Back.Phoenix;
using Data.Piece.Back.Pike;
using Data.Piece.Back.Pistol;
using Data.Piece.Back.Silver;
using Data.Piece.Front.Bow;
using Data.Piece.Front.Captain;
using Data.Piece.Front.Catapult;
using Data.Piece.Front.Commander;
using Data.Piece.Front.Fortress;
using Data.Piece.Front.HiddenDragon;
using Data.Piece.Front.Pawn;
using Data.Piece.Front.Prodigy;
using Data.Piece.Front.Samurai;
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
                // Front-Back Combos
                case PieceType.COMMANDER:
                    return new CommanderData();

                case PieceType.CAPTAIN:
                    return new CaptainData();
                case PieceType.PISTOL:
                    return new PistolData();

                case PieceType.SAMURAI:
                    return new SamuraiData();
                case PieceType.PIKE:
                    return new PikeData();

                case PieceType.SPY:
                    return new SpyData();
                case PieceType.CLANDESTINITE:
                    return new ClandestiniteData();

                case PieceType.CATAPULT:
                    return new CatapultData();
                case PieceType.FORTRESS:
                    return new FortressData();
                case PieceType.LANCE:
                    return new LanceData();

                case PieceType.HIDDEN_DRAGON:
                    return new HiddenDragonData();
                case PieceType.DRAGON_KING:
                    return new DragonKingData();

                case PieceType.PRODIGY:
                    return new ProdigyData();
                case PieceType.PHOENIX:
                    return new PhoenixData();

                case PieceType.BOW:
                    return new BowData();
                case PieceType.ARROW:
                    return new ArrowData();

                case PieceType.PAWN:
                    return new PawnData();
                case PieceType.BRONZE:
                    return new BronzeData();
                case PieceType.SILVER:
                    return new SilverData();
                case PieceType.GOLD:
                    return new GoldData();
                default:
                    throw new InvalidOperationException("Invalid PieceType when creating IPieceData");
            }
        }
    }
}
