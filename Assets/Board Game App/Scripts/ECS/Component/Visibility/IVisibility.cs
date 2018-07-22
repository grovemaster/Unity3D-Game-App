using Svelto.ECS;

namespace ECS.Component.Visibility
{
    public interface IVisibility : IComponent
    {
        DispatchOnSet<bool> IsVisible { get; set; }
    }
}
