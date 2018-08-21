using Data.Step.Piece.Move;
using ECS.EntityView.Piece;
using Service.Piece.Find;
using Service.Piece.Set;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Piece.Move
{
    class MovePieceEngine : IStep<MovePieceStepState>, IQueryingEntitiesEngine
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();

        private readonly ISequencer moveSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public MovePieceEngine(ISequencer moveSequence)
        {
            this.moveSequence = moveSequence;
        }

        public void Ready()
        { }

        public void Step(ref MovePieceStepState token, int condition)
        {
            // TODO Find top piece at PREVIOUS location and set topOfTower = true
            Vector2 previousLocation = token.pieceToMove.Location.Location;

            PieceEV? topPieceCurrentlyAtDestination = pieceFindService.FindTopPieceByLocation(
                token.destinationTile.Location.Location, entitiesDB);
            int currentTowerTier = pieceFindService.FindPiecesByLocation(token.destinationTile.Location.Location, entitiesDB).Count;
            pieceSetService.SetTopOfTowerToFalse(topPieceCurrentlyAtDestination, entitiesDB);

            int newTier = currentTowerTier + 1;

            // Set location.z, topOfTower = false of all pieces at destination tile,
            // set tier of moving piece, set topOfTower = true for moving piece
            var newLocation = token.destinationTile.Location.Location;

            pieceSetService.SetPieceLocationAndTier(token.pieceToMove, newLocation, newTier, entitiesDB);
            token.pieceToMove.MovePiece.NewLocation = newLocation;

            List<PieceEV> piecesPreviousLocation = pieceFindService.FindPiecesByLocation(
                previousLocation, entitiesDB);

            if (piecesPreviousLocation.Count > 0)
            {
                pieceSetService.SetTopOfTower(
                    piecesPreviousLocation[piecesPreviousLocation.Count - 1], entitiesDB);
            }

            var forcedRecoveryToken = new ForcedRecoveryStepState
            {
                pieceMoved = token.pieceToMove,
                pieceCaptured = token.pieceToCapture
            };
            moveSequence.Next(this, ref forcedRecoveryToken);
        }
    }
}
