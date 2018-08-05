using Data.Step.Piece.Move;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Piece.Move
{
    class MovePieceEngine : IStep<MovePieceStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref MovePieceStepState token, int condition)
        {
            // TODO Find top piece at PREVIOUS location and set topOfTower = true
            Vector3 previousLocation = token.pieceToMove.location.Location;

            PieceEV? topPieceCurrentlyAtDestination = PieceService.FindTopPieceByLocation(
                token.destinationTile.location.Location, entitiesDB);
            int currentTowerTier = PieceService.FindPiecesByLocation(token.destinationTile.location.Location, entitiesDB).Count;
            PieceService.SetTopOfTowerToFalse(topPieceCurrentlyAtDestination, entitiesDB);

            int newTier = currentTowerTier + 1;

            // Set location.z, topOfTower = false of all pieces at destination tile,
            // set tier of moving piece, set topOfTower = true for moving piece
            var newLocation = new Vector3(
                token.destinationTile.location.Location.x,
                token.destinationTile.location.Location.y,
                newTier);

            PieceService.SetPieceLocationAndTier(token.pieceToMove, newLocation, newTier, entitiesDB);
            token.pieceToMove.movePiece.NewLocation = newLocation;

            List<PieceEV> piecesPreviousLocation = PieceService.FindPiecesByLocation(
                previousLocation, entitiesDB);

            if (piecesPreviousLocation.Count > 0)
            {
                PieceService.SetTopOfTower(
                    piecesPreviousLocation[piecesPreviousLocation.Count - 1], entitiesDB);
            }
        }
    }
}
