using Data.Enum.Player;
using Data.Step.Piece.Move;
using Data.Step.Turn;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Hand;
using Service.Piece.Set;
using Service.Turn;
using Svelto.ECS;

namespace ECS.Engine.Piece.Ability.ForcedRecovery
{
    class ForcedRecoveryAbilityEngine : IStep<ForcedRecoveryStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();
        private PieceSetService pieceSetService = new PieceSetService();

        private readonly ISequencer forcedRecoverySequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public ForcedRecoveryAbilityEngine(ISequencer forcedRecoverySequence)
        {
            this.forcedRecoverySequence = forcedRecoverySequence;
        }

        public void Ready()
        { }

        public void Step(ref ForcedRecoveryStepState token, int condition)
        {
            MovePawnToHand(token.pieceMoved);

            if (token.pieceCaptured.HasValue)
            {
                MoveCapturePieceToOtherPlayersHand(token.pieceCaptured.Value);
            }

            NextActionTurnEnd();
        }

        private void MovePawnToHand(PieceEV piece)
        {
            pieceSetService.SetPieceLocationToHandLocation(piece, entitiesDB);
            piece.Visibility.IsVisible.value = false;

            handService.AddPieceToHand(piece, entitiesDB);
        }

        private void MoveCapturePieceToOtherPlayersHand(PieceEV pieceCaptured)
        {
            // Remove piece from player hand
            TurnEV currentTurn = TurnService.GetCurrentTurnEV(entitiesDB);
            HandPieceEV handPiece = handService.FindHandPiece(pieceCaptured.Piece.PieceType, currentTurn.TurnPlayer.PlayerColor, entitiesDB);
            handService.DecrementHandPiece(ref handPiece);

            // Add piece to other player's hand
            PlayerColor opponent = TurnService.CalcOtherTurnPlayer(currentTurn.TurnPlayer.PlayerColor);
            handService.AddPieceToHand(pieceCaptured, entitiesDB, opponent);
        }

        private void NextActionTurnEnd()
        {
            var turnEndToken = new TurnEndStepState();
            forcedRecoverySequence.Next(this, ref turnEndToken);
        }
    }
}
