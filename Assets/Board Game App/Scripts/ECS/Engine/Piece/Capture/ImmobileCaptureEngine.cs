using Data.Step.Piece.Capture;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Piece.Capture
{
    class ImmobileCaptureEngine : IStep<ImmobileCapturePieceStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref ImmobileCapturePieceStepState token, int condition)
        {
            PieceEV pieceToCapture = token.pieceToCapture;
            Vector2 towerLocation = new Vector2(pieceToCapture.Location.Location.x, pieceToCapture.Location.Location.y);
            PieceService.SetPieceLocationToHandLocation(pieceToCapture, entitiesDB);
            pieceToCapture.Visibility.IsVisible.value = false;

            AdjustRemainingTowerPieces(towerLocation);
        }

        private void AdjustRemainingTowerPieces(Vector2 towerLocation)
        {
            List<PieceEV> pieces = PieceService.FindPiecesByLocation(towerLocation, entitiesDB);

            for (int i = 0; i < pieces.Count; ++i)
            {
                PieceService.SetPieceLocationAndTier(pieces[i], pieces[i].Location.Location, i + 1, entitiesDB);
                PieceService.SetTopOfTower(pieces[i], entitiesDB, i == pieces.Count - 1);

                pieces[i].MovePiece.NewLocation = pieces[i].Location.Location;
            }
        }
    }
}
