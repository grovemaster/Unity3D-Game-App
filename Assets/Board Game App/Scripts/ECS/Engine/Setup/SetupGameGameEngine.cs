using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Enums.Piece;
using Data.Enums.Player;
using Data.Step.Turn;
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
using UnityEngine;

namespace ECS.Engine.Setup
{
    class SetupGameGameEngine : SingleEntityEngine<TitleEV>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        private readonly ISequencer setupGameSequencer;

        private bool isMobile;
        private string persistentDataPath;

        private HandService handService = new HandService();
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private TurnService turnService = new TurnService();

        public SetupGameGameEngine(
            ISequencer setupGameSequencer,
            bool isMobile,
            string persistentDataPath)
        {
            this.setupGameSequencer = setupGameSequencer;
            this.isMobile = isMobile;
            this.persistentDataPath = persistentDataPath;
        }

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
                GotoTurnStartStep();
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
            string fileName = "saved_game.txt";
            if (isMobile)
            {
                fileName = Path.Combine(persistentDataPath, fileName);
            }

            // For simplicity, I'm not error checking this file.
            string[] lines = File.ReadAllLines(fileName);
            SetTurnStatus(lines[0]);

            int index = 1;
            while (index < lines.Length && IsHandPieceLine(lines[index])) {
                string[] data = lines[index].Split(',');
                MovePiecesToHand(data);

                index++;
            }

            List<PieceEV> pieces = pieceFindService.FindAllBoardPieces(entitiesDB).ToList();

            for (; index < lines.Length; ++index)
            {
                string[] data = lines[index].Split(',');
                PieceEV pieceToRemoveFromList = PositionPiece(data, pieces);
                pieces.Remove(pieceToRemoveFromList); // Prevent future line from re-position already-positioned piece
            }
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

        private void SetTurnStatus(string line)
        {
            string[] data = line.Split(',');
            bool initialArrangementInEffect = data[0].Equals("1") ? true : false;
            PlayerColor turnPlayer = ConvertToPlayerColor(data[1]);

            SetTurnStatus(turnPlayer, initialArrangementInEffect);
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

        private bool IsHandPieceLine(string line)
        {
            return line.Contains("HAND PIECE");
        }

        private void MovePiecesToHand(string[] data)
        {
            PlayerColor teamColor = ConvertToPlayerColor(data[0]);
            PieceType front = ConvertToPieceType(data[2]);
            PieceType back = ConvertToPieceType(data[3]);
            int numPiecesToHand = Convert.ToInt32(data[4]);
            PieceEV[] pieces = pieceFindService.FindPiecesByType(front, back, entitiesDB);

            for (int i = 0; i < numPiecesToHand; ++i)
            {
                handService.AddPieceToHand(pieces[i], entitiesDB, teamColor);
                pieceSetService.SetPieceLocationToHandLocation(pieces[i], entitiesDB);
                pieces[i].Visibility.IsVisible.value = false;
            }
        }

        private PieceEV PositionPiece(string[] data, List<PieceEV> pieces)
        {
            PlayerColor teamColor = ConvertToPlayerColor(data[0]);
            PieceType pieceType = ConvertToPieceType(data[2]);
            PieceType front = ConvertToPieceType(data[3]);
            PieceType back = ConvertToPieceType(data[4]);
            int tier = Convert.ToInt32(data[5]);
            bool topOfTower = data[6] == "1" ? true : false;
            Vector2 location = new Vector2(Convert.ToInt32(data[7]), Convert.ToInt32(data[8]));
            PieceEV pieceToPosition = pieces.Find(piece =>
                piece.Piece.Front == front && piece.Piece.Back == back);

            pieceSetService.SetPiecePlayerOwner(pieceToPosition, teamColor, entitiesDB);
            pieceSetService.SetPieceLocationAndTier(pieceToPosition, location, tier, entitiesDB);
            pieceSetService.SetTopOfTower(pieceToPosition, entitiesDB, topOfTower);
            pieceToPosition.MovePiece.NewLocation = location;
            pieceToPosition.ChangeColorTrigger.PlayChangeColor = true;

            return pieceToPosition;
        }

        private PlayerColor ConvertToPlayerColor(string input)
        {
            return (PlayerColor)Enum.Parse(typeof(PlayerColor), input);
        }

        private PieceType ConvertToPieceType(string input)
        {
            return (PieceType)Enum.Parse(typeof(PieceType), input);
        }

        private void GotoTurnStartStep()
        {
            var token = new TurnStartStepState();
            setupGameSequencer.Next(this, ref token);
        }
    }
}
