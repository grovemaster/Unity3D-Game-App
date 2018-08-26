using Data.Enum.Player;
using ECS.EntityView.Turn;
using Service.Common;
using Svelto.ECS;

namespace Service.Turn
{
    public class TurnService
    {
        public static PlayerColor CalcOtherTurnPlayer(PlayerColor playerColor)
        {
            return playerColor == PlayerColor.BLACK ? PlayerColor.WHITE : PlayerColor.BLACK;
        }

        public void SwitchTurnPlayer(IEntitiesDB entitiesDB)
        {
            TurnEV currentTurn = GetCurrentTurnEV(entitiesDB);
            SetTurnEV(currentTurn, entitiesDB);
        }

        public TurnEV GetCurrentTurnEV(IEntitiesDB entitiesDB)
        {
            TurnEV[] turnEVs = CommonService.FindAllEntities<TurnEV>(entitiesDB);

            // The correct TurnEV should always be first.
            return turnEVs[0];
        }

        public void SetCheckStatus(TurnEV currentTurn, bool inCheck, IEntitiesDB entitiesDB)
        {
            entitiesDB.ExecuteOnEntity(
                currentTurn.ID,
                (ref TurnEV turnToChange) => { turnToChange.Check.CommanderInCheck = inCheck; });
        }

        public bool IsRankWithinTerritory(TurnEV currentTurn, float rank)
        {
            return (currentTurn.TurnPlayer.PlayerColor == PlayerColor.BLACK && rank < 3)
                || (currentTurn.TurnPlayer.PlayerColor == PlayerColor.WHITE && rank > 5);
        }

        private void SetTurnEV(TurnEV currentTurn, IEntitiesDB entitiesDB)
        {
            PlayerColor nextTurnPlayer =
                currentTurn.TurnPlayer.PlayerColor == PlayerColor.BLACK
                ? PlayerColor.WHITE : PlayerColor.BLACK;

            entitiesDB.ExecuteOnEntity(
                currentTurn.ID,
                (ref TurnEV turnToChange) => { turnToChange.TurnPlayer.PlayerColor = nextTurnPlayer; });
        }
    }
}
