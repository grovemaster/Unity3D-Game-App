using System;
using System.Collections.Generic;
using System.Linq;
using Data.Enums.Piece;
using Data.Enums.Player;
using ECS.EntityView.Hand;
using ECS.EntityView.Menu;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Hand;
using Service.Piece.Find;
using Service.Piece.Set;
using Service.Turn;
using Svelto.ECS;
using UI.GameState;

namespace ECS.Engine.Setup
{
    class SetupGameGameEngine : SingleEntityEngine<TitleEV>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        private HandService handService = new HandService();
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private TurnService turnService = new TurnService();

        public void Ready()
        { }

        protected override void Add(ref TitleEV entityView)
        {
            // EV is unimportant
            // Figure out a more elegant solution later
            if (LoadOrNew.ContinueOrNew.Equals("New"))
            {
                SetupNewGame();
            }
            else
            {
                LoadSavedGame();
            }
        }

        protected override void Remove(ref TitleEV entityView)
        {
        }

        private void SetupNewGame()
        {
            SetTurnStatus(PlayerColor.BLACK, true);
            MoveAllPiecesToHand();
        }

        private void LoadSavedGame()
        {
            throw new NotImplementedException();
        }

        private void SetTurnStatus(PlayerColor turnPlayer, bool initialArrangementInEffect)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);

            entitiesDB.ExecuteOnEntity(
                currentTurn.ID,
                (ref TurnEV turnToChange) =>
                {
                    turnToChange.TurnPlayer.PlayerColor = turnPlayer;
                    turnToChange.InitialArrangement.IsInitialArrangementInEffect = initialArrangementInEffect;
                });
        }

        private void MoveAllPiecesToHand()
        {
            PieceEV[] pieces = pieceFindService.FindAllBoardPieces(entitiesDB);

            MovePieceTypeToHand(PieceType.PAWN, PieceType.BRONZE, pieces);
            MovePieceTypeToHand(PieceType.PAWN, PieceType.SILVER, pieces);
            MovePieceTypeToHand(PieceType.PAWN, PieceType.GOLD, pieces);
            MovePieceTypeToHand(PieceType.SPY, PieceType.CLANDESTINITE, pieces);
            MovePieceTypeToHand(PieceType.CATAPULT, PieceType.LANCE, pieces);
            MovePieceTypeToHand(PieceType.FORTRESS, PieceType.LANCE, pieces);
            MovePieceTypeToHand(PieceType.HIDDEN_DRAGON, PieceType.DRAGON_KING, pieces);
            MovePieceTypeToHand(PieceType.PRODIGY, PieceType.PHOENIX, pieces);
            MovePieceTypeToHand(PieceType.BOW, PieceType.ARROW, pieces);
            MovePieceTypeToHand(PieceType.SAMURAI, PieceType.PIKE, pieces);
            MovePieceTypeToHand(PieceType.CAPTAIN, PieceType.PISTOL, pieces);
            MovePieceTypeToHand(PieceType.COMMANDER, PieceType.COMMANDER, pieces);
        }

        private void MovePieceTypeToHand(PieceType front, PieceType back, PieceEV[] pieces)
        {
            PieceEV[] piecesOfType = pieces.Where(piece =>
                piece.Piece.Front == front && piece.Piece.Back == back).ToArray();
            MovePieceTypeToHand(PlayerColor.BLACK, front, back, piecesOfType);
            MovePieceTypeToHand(PlayerColor.WHITE, front, back, piecesOfType);
        }

        private void MovePieceTypeToHand(PlayerColor teamColor, PieceType front, PieceType back, PieceEV[] pieces)
        {
            HandPieceEV handPiece = handService.FindHandPiece(front, back, teamColor, entitiesDB);
            List<PieceEV> teamPieces = pieces.Where(piece => piece.PlayerOwner.PlayerColor == teamColor).ToList();

            teamPieces.ForEach(piece =>
            {
                handService.AddPieceToHand(piece, entitiesDB, teamColor);
                pieceSetService.SetPieceLocationToHandLocation(piece, entitiesDB);
                piece.Visibility.IsVisible.value = false;
            });
        }
    }
}
