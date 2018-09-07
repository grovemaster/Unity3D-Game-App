using Data.Step.Turn;
using ECS.EntityView.Turn;
using Service.Checkmate;
using Service.Turn;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Engine.Checkmate
{
    class CheckmateEngine : IStep<TurnStartStepState>, IQueryingEntitiesEngine
    {
        private CheckmateService checkmateService = new CheckmateService();
        private TurnService turnService = new TurnService();

        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready()
        { }

        public void Step(ref TurnStartStepState token, int condition)
        {
            TurnEV turn = turnService.GetCurrentTurnEV(entitiesDB);

            if (turn.Check.CommanderInCheck && !checkmateService.AnyValidMoves(turn, entitiesDB))
            {
                Debug.Log("CHECKMATED PLAYER: " + turn.TurnPlayer.PlayerColor.ToString());
            }
        }
    }
}
