using Svelto.ECS;
using ECS.Component.Player;

namespace ECS.EntityView.Turn
{
    public struct TurnEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IPlayerComponent TurnPlayer;
    }
}
