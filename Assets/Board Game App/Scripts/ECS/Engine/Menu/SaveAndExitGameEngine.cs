using ECS.EntityView.Hand;
using ECS.EntityView.Menu;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Hand;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;
using System;
using System.IO;

namespace ECS.Engine.Menu
{
    class SaveAndExitGameEngine : SingleEntityEngine<TitleEV>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        private Action gotoTitleScreen;

        private HandService handService = new HandService();
        private PieceFindService pieceFindService = new PieceFindService();
        private TurnService turnService = new TurnService();

        public void Ready()
        { }

        protected override void Add(ref TitleEV entityView)
        {
            this.gotoTitleScreen = entityView.Title.ClickAction;
            entityView.Title.Clicked.NotifyOnValueSet(OnPressed);
        }

        protected override void Remove(ref TitleEV entityView)
        {
            entityView.Title.Clicked.StopNotify(OnPressed);
        }

        private void OnPressed(int entityId, bool isPressed)
        {
            if (!isPressed)
            {
                return;
            }

            SaveGameToFile();
            GotoTitleScreen();
        }

        private void SaveGameToFile()
        {
            // TODO Save game
            HandPieceEV[] handPieces = handService.FindAllHandPieces(entitiesDB);
            PieceEV[] pieces = pieceFindService.FindAllBoardPieces(entitiesDB);

            using (var tw = new StreamWriter("saved_game.txt", false))
            {
                tw.WriteLine(CreateTurnSaveInfoString());

                for (int i = 0; i < handPieces.Length; ++i)
                {
                    if (handPieces[i].HandPiece.NumPieces.value > 0)
                    {
                        tw.WriteLine(CreateHandPieceSaveInfoString(handPieces[i]));
                    }
                }

                for (int i = 0; i < pieces.Length; ++i)
                {
                    tw.WriteLine(CreatePieceSaveInfoString(pieces[i]));
                }
            }
        }

        private void GotoTitleScreen()
        {
            gotoTitleScreen.Invoke();
        }

        private string CreateTurnSaveInfoString()
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            int iaActive = currentTurn.InitialArrangement.IsInitialArrangementInEffect ? 1 : 0;

            return iaActive + "," + currentTurn.TurnPlayer.PlayerColor.ToString();
        }

        private string CreateHandPieceSaveInfoString(HandPieceEV handPiece)
        {
            return handPiece.PlayerOwner.PlayerColor + ","
                + "HAND PIECE,"
                + handPiece.HandPiece.PieceType.ToString() + ","
                + handPiece.HandPiece.Back.ToString() + ","
                + handPiece.HandPiece.NumPieces.value;
        }

        private string CreatePieceSaveInfoString(PieceEV piece)
        {
            int topOfTower = piece.Tier.TopOfTower ? 1 : 0;

            return piece.PlayerOwner.PlayerColor + ","
                + "PIECE,"
                + piece.Piece.PieceType.ToString() + ","
                + piece.Piece.Back.ToString() + ","
                + piece.Piece.Back.ToString() + ","
                + piece.Tier.Tier + ","
                + topOfTower + ","
                + piece.Location.Location.x + ","
                + piece.Location.Location.y;
        }
    }
}
