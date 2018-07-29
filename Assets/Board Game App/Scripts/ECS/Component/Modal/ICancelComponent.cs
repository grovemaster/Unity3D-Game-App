using Svelto.ECS;

namespace ECS.Component.Modal
{
    public interface ICancelComponent
    {
        DispatchOnSet<bool> Cancel { get; set; } // Engine listens for cancel
    }
}
