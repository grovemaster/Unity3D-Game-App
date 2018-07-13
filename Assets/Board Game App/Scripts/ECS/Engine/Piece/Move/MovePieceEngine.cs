using Data.Step.Piece.Move;
using ECS.EntityView.Piece;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Engine.Piece.Move
{
    class MovePieceEngine : IStep<MovePieceInfo>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref MovePieceInfo token, int condition)
        {
            var newLocation = new Vector3(
                token.destinationTile.location.Location.x,
                token.destinationTile.location.Location.y,
                token.pieceToMove.location.Location.z);

            entitiesDB.ExecuteOnEntity(token.pieceToMove.ID,
                (ref PieceEV pieceEV) => { pieceEV.location.Location = newLocation; } );

            token.pieceToMove.movePiece.NewLocation = newLocation;
        }
    }
}
