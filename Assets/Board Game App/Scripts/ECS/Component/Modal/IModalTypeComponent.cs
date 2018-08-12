using Data.Enum.Modal;

namespace ECS.Component.Modal
{
    public interface IModalTypeComponent : IComponent
    {
        ModalType Type { get; set; }
    }
}
