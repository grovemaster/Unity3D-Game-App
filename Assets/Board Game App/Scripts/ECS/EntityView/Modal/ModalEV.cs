﻿using ECS.Component.Modal;
using ECS.Component.Player;
using ECS.Component.Visibility;
using Svelto.ECS;

namespace ECS.EntityView.Modal
{
    public struct ModalEV : IEntityViewStruct
    {
        public EGID ID { get; set; }

        public IModalTypeComponent Type;
        public ICaptureOrStackComponent CaptureOrStack;
        public IDropFrontBackComponent DropFrontBackModal;
        public IAnswerComponent Answer;
        public ICancelComponent Cancel;
        public IConfirmComponent Confirm;
        public IPlayerComponent VictoriousPlayer;
        public IVisibilityComponent Visibility;
    }
}
