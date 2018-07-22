using Svelto.ECS;
using ECS.Component.Hand;
using ECS.Component.Player;

namespace ECS.EntityView.Hand
{
    public struct HandPieceEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IHandPieceComponent handPiece;
        public IPlayerComponent playerOwner;
    }
}
