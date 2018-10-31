using ECS.Component.Menu;
using Svelto.ECS;

namespace ECS.EntityView.Menu
{
    public struct TitleEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public ITitleComponent Title;
    }
}
