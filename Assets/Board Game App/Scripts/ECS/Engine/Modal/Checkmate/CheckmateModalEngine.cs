using Data.Enums.Modal;
using Data.Enums.Player;
using Data.Step.Modal.Checkmate;
using ECS.EntityView.Modal;
using ECS.EntityView.Turn;
using Service.Modal;
using Service.Turn;
using Svelto.ECS;

namespace ECS.Engine.Modal.Checkmate
{
    class CheckmateModalEngine : IStep<CheckmateModalStepState>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();
        private TurnService turnService = new TurnService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref CheckmateModalStepState token, int condition)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            PlayerColor victoriousPlayer = TurnService.CalcOtherTurnPlayer(currentTurn.TurnPlayer.PlayerColor);

            SetModalOptions(modal, victoriousPlayer);
            modalService.DisplayModal(modal);
        }

        private void SetModalOptions(ModalEV modal, PlayerColor victoriousPlayer)
        {
            SetModalType(modal, ModalType.CHECKMATE);
            SetModalVictoriousPlayer(modal, victoriousPlayer);
        }

        // TODO Move all usages to ModalService
        private void SetModalType(ModalEV modal, ModalType modalType)
        {
            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modalToChange.Type.Type = modalType;
                });
        }

        private void SetModalVictoriousPlayer(ModalEV modal, PlayerColor victoriousPlayer)
        {
            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modalToChange.VictoriousPlayer.PlayerColor = victoriousPlayer;
                });
        }
    }
}
