using Svelto.ECS;
using ECS.Component.Hand;
using ECS.Component.Player;
using ECS.Component.SharedComponent;

namespace ECS.EntityView.Hand
{
    public struct HandPieceEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IHandPieceComponent handPiece;
        public IPlayerComponent playerOwner;
        public IHighlightComponent highlight;
        public IChangeColorComponent changeColor;
    }
}
