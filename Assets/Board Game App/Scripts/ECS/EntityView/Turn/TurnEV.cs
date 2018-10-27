using Svelto.ECS;
using ECS.Component.Player;
using ECS.Component.InitialArrangement;

namespace ECS.EntityView.Turn
{
    public struct TurnEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IPlayerComponent TurnPlayer;
        public ICheckComponent Check;
        public IForcedRearrangementComponent ForcedRearrangementStatus;
        public IInitialArrangementComponent InitialArrangement;
    }
}
