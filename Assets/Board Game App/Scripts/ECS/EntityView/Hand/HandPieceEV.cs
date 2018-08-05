using Svelto.ECS;
using ECS.Component.Hand;
using ECS.Component.Player;
using ECS.Component.SharedComponent;

namespace ECS.EntityView.Hand
{
    public struct HandPieceEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IHandPieceComponent HandPiece;
        public IPlayerComponent PlayerOwner;
        public IHighlightComponent Highlight;
        public IChangeColorComponent ChangeColorTrigger;
    }
}
