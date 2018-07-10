using Svelto.ECS;
using ECS.Component.Piece;
using ECS.Component.SharedComponent;

namespace ECS.EntityView.Piece
{
    public struct PieceEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IPiece piece;
        public ILocation location;
        public IHighlight highlight;
    }
}
