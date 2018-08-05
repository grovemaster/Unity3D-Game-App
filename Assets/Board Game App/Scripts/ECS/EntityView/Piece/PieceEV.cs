using Svelto.ECS;
using ECS.Component.Piece;
using ECS.Component.SharedComponent;
using ECS.Component.Piece.Move;
using ECS.Component.Player;
using ECS.Component.Visibility;
using ECS.Component.Piece.Tower;

namespace ECS.EntityView.Piece
{
    public struct PieceEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IPieceComponent Piece;
        public IMovePieceComponent MovePiece;
        public ILocationComponent Location;
        public ITowerTierComponent Tier;
        public IHighlightComponent Highlight;
        public IChangeColorComponent ChangeColorTrigger;
        public IPlayerComponent PlayerOwner;
        public IVisibilityComponent Visibility;
    }
}
