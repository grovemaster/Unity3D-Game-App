using ECS.Component.Modal;
using ECS.Component.Player;
using ECS.Component.Visibility;
using Svelto.ECS;

namespace ECS.EntityView.Modal
{
    public struct ModalEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IModalTypeComponent Type;
        public ITier1OptionComponent Tier1;
        public ITier2OptionComponent Tier2;
        public ITier3OptionComponent Tier3;
        public ICaptureOrStackComponent CaptureOrStack;
        public IDropFrontBackComponent DropFrontBackModal;
        public IAnswerComponent Answer;
        public ICancelComponent Cancel;
        public IConfirmComponent Confirm;
        public IImmobileCaptureStateComponent ImmobileCaptureState;
        public IPlayerComponent VictoriousPlayer;
        public IVisibilityComponent Visibility;
    }
}
