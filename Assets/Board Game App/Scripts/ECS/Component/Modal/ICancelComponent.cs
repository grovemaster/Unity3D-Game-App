using Svelto.ECS;

namespace ECS.Component.Modal
{
    public interface ICancelComponent : IComponent
    {
        DispatchOnSet<bool> Cancel { get; set; } // Engine listens for cancel
    }
}
