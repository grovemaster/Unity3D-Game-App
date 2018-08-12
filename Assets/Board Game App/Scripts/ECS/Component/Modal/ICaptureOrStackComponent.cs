using Data.Enum.Modal;
using Svelto.ECS;

namespace ECS.Component.Modal
{
    public interface ICaptureOrStackComponent : IComponent
    {
        int TileReferenceId { get; set; }
        DispatchOnSet<ModalQuestionAnswer> Answer { get; set; }
    }
}
