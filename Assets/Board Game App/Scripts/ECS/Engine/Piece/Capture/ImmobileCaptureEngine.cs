using Data.Enums.Piece.PostMove;
using Data.Enums.Player;
using Data.Piece.Map;
using Data.Step.Piece.Capture;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Piece.Find;
using Service.Piece.ImmobileCapture;
using Service.Piece.Set;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Piece.Capture
{
    class ImmobileCaptureEngine : IStep<ImmobileCapturePieceStepState>, IQueryingEntitiesEngine
    {
        private ImmobileCaptureService immobileCaptureService = new ImmobileCaptureService();
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();
        private TurnService turnService = new TurnService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref ImmobileCapturePieceStepState token, int condition)
        {
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            PieceEV pieceToCapture = token.PieceToCapture;
            Vector2 towerLocation = new Vector2(pieceToCapture.Location.Location.x, pieceToCapture.Location.Location.y);
            bool isBetrayalPossible = !immobileCaptureService.IsFriendlyBetrayalTopOfTower(towerLocation, currentTurn.TurnPlayer.PlayerColor, entitiesDB);
            pieceSetService.SetPieceLocationToHandLocation(pieceToCapture, entitiesDB);
            pieceToCapture.Visibility.IsVisible.value = false;

            AdjustRemainingTowerPieces(towerLocation);
            // Betrayal possible if friendly betrayal piece BECOMES topOfTower
            isBetrayalPossible = isBetrayalPossible && immobileCaptureService.IsFriendlyBetrayalTopOfTower(towerLocation, currentTurn.TurnPlayer.PlayerColor, entitiesDB);
            token.BetrayalPossible = isBetrayalPossible;

            if (token.BetrayalPossible)
            {
                // If friendly betrayal piece becomes topOfTower, by deduction, it's the pieceToStrike
                token.pieceToStrike = pieceFindService.FindTopPieceByLocation(towerLocation, entitiesDB).Value;
            }
        }

        private void AdjustRemainingTowerPieces(Vector2 towerLocation)
        {
            List<PieceEV> towerPieces = pieceFindService.FindPiecesByLocation(towerLocation, entitiesDB);

            for (int i = 0; i < towerPieces.Count; ++i)
            {
                pieceSetService.SetPieceLocationAndTier(towerPieces[i], towerPieces[i].Location.Location, i + 1, entitiesDB);
                pieceSetService.SetTopOfTower(towerPieces[i], entitiesDB, i == towerPieces.Count - 1);

                towerPieces[i].MovePiece.NewLocation = towerPieces[i].Location.Location;
            }
        }
    }
}
