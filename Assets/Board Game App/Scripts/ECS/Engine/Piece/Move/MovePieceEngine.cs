using Data.Step.Piece.Move;
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
            var newLocation = new Vector3(
                token.destinationTile.location.Location.x,
                token.destinationTile.location.Location.y,
                token.pieceToMove.location.Location.z);

            PieceService.SetPieceLocation(token.pieceToMove, newLocation, entitiesDB);
            token.pieceToMove.movePiece.NewLocation = newLocation;
        }
    }
}
