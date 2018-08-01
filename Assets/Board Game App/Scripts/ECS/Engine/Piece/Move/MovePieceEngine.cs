using Data.Step.Piece.Move;
using ECS.EntityView.Piece;
using Service.Piece;
using Svelto.ECS;
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
            PieceEV? topPieceCurrentlyAtDestination = PieceService.FindTopPieceByLocation(
                token.destinationTile.location.Location, entitiesDB);
            PieceService.SetTopOfTowerToFalse(topPieceCurrentlyAtDestination, entitiesDB);

            int newTier = topPieceCurrentlyAtDestination.HasValue ?
                topPieceCurrentlyAtDestination.Value.tier.Tier + 1 : 1;

            // Set location.z, topOfTower = false of all pieces at destination tile,
            // set tier of moving piece, set topOfTower = true for moving piece
            var newLocation = new Vector3(
                token.destinationTile.location.Location.x,
                token.destinationTile.location.Location.y,
                newTier);

            PieceService.SetPieceLocationAndTier(token.pieceToMove, newLocation, newTier, entitiesDB);
            token.pieceToMove.movePiece.NewLocation = newLocation;
        }
    }
}
