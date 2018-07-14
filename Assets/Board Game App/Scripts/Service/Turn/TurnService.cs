using Data.Enum.Player;
using ECS.EntityView.Turn;
using Service.Common;
using Svelto.ECS;

namespace Service.Turn
{
    public static class TurnService
    {
        public static void SwitchTurnPlayer(IEntitiesDB entitiesDB)
        {
            TurnEV currentTurn = GetCurrentTurnEV(entitiesDB);
            SetTurnEV(currentTurn, entitiesDB);
        }

        public static TurnEV GetCurrentTurnEV(IEntitiesDB entitiesDB)
        {
            int count;
            TurnEV[] turnEVs = CommonService.FindAllEntities<TurnEV>(entitiesDB, out count);

            // The correct TurnEV should always be first.
            return turnEVs[0];
        }

        private static void SetTurnEV(TurnEV currentTurn, IEntitiesDB entitiesDB)
        {
            PlayerColor nextTurnPlayer =
                currentTurn.TurnPlayer.PlayerColor.Equals(PlayerColor.BLACK)
                ? PlayerColor.WHITE : PlayerColor.BLACK;

            entitiesDB.ExecuteOnEntity(
                currentTurn.ID,
                (ref TurnEV turnToChange) => { turnToChange.TurnPlayer.PlayerColor = nextTurnPlayer; });
        }
    }
}
