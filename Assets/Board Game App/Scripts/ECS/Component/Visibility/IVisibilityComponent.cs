using Svelto.ECS;

namespace ECS.Component.Visibility
{
    public interface IVisibilityComponent : IComponent
    {
        DispatchOnSet<bool> IsVisible { get; set; }
    }
}
