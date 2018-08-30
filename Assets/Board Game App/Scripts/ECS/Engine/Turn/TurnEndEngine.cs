using Data.Step.Turn;
using Service.Turn;
using Svelto.ECS;

namespace ECS.Engine.Turn
{
    class TurnEndEngine : IStep<TurnEndStepState>, IQueryingEntitiesEngine
    {
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

            var turnStartStepState = new TurnStartStepState();
            turnStartSequencer.Next(this, ref turnStartStepState);
        }

        private void SwitchTurnPlayer()
        {
            turnService.SwitchTurnPlayer(entitiesDB);
        }
    }
}
