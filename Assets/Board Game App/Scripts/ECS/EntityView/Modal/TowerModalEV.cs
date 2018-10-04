using ECS.Component.Modal;
using ECS.Component.Visibility;
using Svelto.ECS;

namespace ECS.EntityView.Modal
{
    public struct TowerModalEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IModalTypeComponent Type;
        public ITier1OptionComponent Tier1;
        public ITier2OptionComponent Tier2;
        public ITier3OptionComponent Tier3;
        public IAnswerComponent Answer;
        public ICancelComponent Cancel;
        public IImmobileCaptureStateComponent ImmobileCaptureState;
        public IVisibilityComponent Visibility;
    }
}
