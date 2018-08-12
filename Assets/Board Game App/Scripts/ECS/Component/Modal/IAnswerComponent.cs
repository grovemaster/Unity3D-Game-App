using Svelto.ECS;

namespace ECS.Component.Modal
{
    public interface IAnswerComponent : IComponent
    {
        DispatchOnSet<int> Answer { get; set; } // Engine listens for valid user button press
    }
}
