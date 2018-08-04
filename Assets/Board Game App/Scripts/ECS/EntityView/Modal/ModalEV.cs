using ECS.Component.Modal;
using ECS.Component.Visibility;
using Svelto.ECS;

namespace ECS.EntityView.Modal
{
    public struct ModalEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IModalTypeComponent type;
        public ITier1OptionComponent tier1;
        public ITier2OptionComponent tier2;
        public ITier3OptionComponent tier3;
        public ICaptureOrStackComponent captureOrStack;
        public IAnswerComponent answer;
        public ICancelComponent cancel;
        public IVisibilityComponent visibility;
    }
}
