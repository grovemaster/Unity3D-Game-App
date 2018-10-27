using Data.Step.Turn;
using ECS.EntityView.Turn;
using Service.Hand;
using Service.Turn;
using Svelto.ECS;

namespace ECS.Engine.Turn
{
    class TurnEndEngine : IStep<TurnEndStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();
        private TurnService turnService = new TurnService();

        private Sequencer turnStartSequencer;

        public IEntitiesDB entitiesDB { private get; set; }

        public TurnEndEngine(Sequencer turnStartSequencer)
        {
            this.turnStartSequencer = turnStartSequencer;
        }

        public void Ready()
        { }

        public void Step(ref TurnEndStepState token, int condition)
        {
            EndTurn();
        }

        private void EndTurn()
        {
            SwitchTurnPlayer();
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            SetInitialArrangementStatus(currentTurn);

            if (!currentTurn.InitialArrangement.IsInitialArrangementInEffect)
            {
                var turnStartStepState = new TurnStartStepState();
                turnStartSequencer.Next(this, ref turnStartStepState);
            }
        }

        private void SwitchTurnPlayer()
        {
            turnService.SwitchTurnPlayer(entitiesDB);
        }

        private void SetInitialArrangementStatus(TurnEV currentTurn)
        {
            if (currentTurn.InitialArrangement.IsInitialArrangementInEffect
                && handService.AreHandsEmpty(entitiesDB))
            {
                entitiesDB.ExecuteOnEntity(
                    currentTurn.ID,
                    (ref TurnEV turnToChange) => { turnToChange.InitialArrangement.IsInitialArrangementInEffect = false; });
            }
        }
    }
}
