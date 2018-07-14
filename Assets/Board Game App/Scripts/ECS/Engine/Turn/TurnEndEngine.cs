using Data.Step.Piece.Move;
using Service.Turn;
using Svelto.ECS;

namespace ECS.Engine.Turn
{
    class TurnEndEngine : IStep<MovePieceStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref MovePieceStepState token, int condition)
        {
            SwitchTurnPlayer();
        }

        private void SwitchTurnPlayer()
        {
            TurnService.SwitchTurnPlayer(entitiesDB);
        }
    }
}
