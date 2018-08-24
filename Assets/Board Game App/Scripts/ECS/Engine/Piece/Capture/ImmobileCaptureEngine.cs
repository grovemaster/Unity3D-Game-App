using Data.Step.Piece.Capture;
using ECS.EntityView.Piece;
using Service.Piece.Find;
using Service.Piece.Set;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Piece.Capture
{
    class ImmobileCaptureEngine : IStep<ImmobileCapturePieceStepState>, IQueryingEntitiesEngine
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref ImmobileCapturePieceStepState token, int condition)
        {
            PieceEV pieceToCapture = token.PieceToCapture;
            Vector2 towerLocation = new Vector2(pieceToCapture.Location.Location.x, pieceToCapture.Location.Location.y);
            pieceSetService.SetPieceLocationToHandLocation(pieceToCapture, entitiesDB);
            pieceToCapture.Visibility.IsVisible.value = false;

            AdjustRemainingTowerPieces(towerLocation);
        }

        private void AdjustRemainingTowerPieces(Vector2 towerLocation)
        {
            List<PieceEV> pieces = pieceFindService.FindPiecesByLocation(towerLocation, entitiesDB);

            for (int i = 0; i < pieces.Count; ++i)
            {
                pieceSetService.SetPieceLocationAndTier(pieces[i], pieces[i].Location.Location, i + 1, entitiesDB);
                pieceSetService.SetTopOfTower(pieces[i], entitiesDB, i == pieces.Count - 1);

                pieces[i].MovePiece.NewLocation = pieces[i].Location.Location;
            }
        }
    }
}
