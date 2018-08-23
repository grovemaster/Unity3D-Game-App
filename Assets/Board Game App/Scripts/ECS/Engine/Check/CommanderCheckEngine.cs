using Data.Step.Turn;
using ECS.EntityView.Turn;
using Service.Check;
using Service.Turn;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Engine.Check
{
    class CommanderCheckEngine : IStep<TurnStartStepState>, IQueryingEntitiesEngine
    {
        private CheckService checkService = new CheckService();
        private TurnService turnService = new TurnService();

        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready()
        { }

        public void Step(ref TurnStartStepState token, int condition)
        {
            TurnEV turn = turnService.GetCurrentTurnEV(entitiesDB);
            bool inCheck = checkService.IsCommanderInCheck(turn.TurnPlayer.PlayerColor, entitiesDB);
            turnService.SetCheckStatus(turn, inCheck, entitiesDB);

            Debug.Log("Turn Player: " + turn.TurnPlayer.PlayerColor.ToString());
            if (inCheck)
            {
                Debug.Log("CHECK: " + turn.TurnPlayer.PlayerColor.ToString());
            }
        }
    }
}
