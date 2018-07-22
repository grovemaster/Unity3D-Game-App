using Svelto.ECS;
using ECS.Component.Piece;
using ECS.Component.SharedComponent;
using ECS.Component.Piece.Move;
using ECS.Component.Player;
using ECS.Component.Visibility;

namespace ECS.EntityView.Piece
{
    public struct PieceEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IPiece piece;
        public IMovePiece movePiece;
        public ILocation location;
        public IHighlight highlight;
        public IChangeColorComponent changeColorComponent;
        public IPlayerComponent playerOwner;
        public IVisibility visibility;
    }
}
