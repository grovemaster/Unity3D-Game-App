using Data.Enum.Modal;
using Svelto.ECS;

namespace ECS.Component.Modal
{
    public interface ICaptureOrStackComponent
    {
        DispatchOnSet<ModalQuestionAnswer> Answer { get; set; }
    }
}
