using Data.Constants.Board;
using Data.Enum.Player;
using Data.Step.Drop;
using Data.Step.Turn;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using Service.Hand;
using Service.Piece.Find;
using Service.Piece.Set;
using Svelto.ECS;
using UnityEngine;

namespace ECS.Engine.Drop
{
    class DropEngine : IStep<DropStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();
        private PieceFindService pieceFindService = new PieceFindService();
        private PieceSetService pieceSetService = new PieceSetService();

        private readonly ISequencer dropSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public DropEngine(ISequencer dropSequence)
        {
            this.dropSequence = dropSequence;
        }

        public void Ready()
        { }

        public void Step(ref DropStepState token, int condition)
        {
            PieceEV pieceToDrop = pieceFindService.FindFirstPieceByLocationAndType(
                BoardConst.HAND_LOCATION, token.handPiece.HandPiece.PieceType, entitiesDB);

            DropPiece(ref pieceToDrop, ref token.destinationTile, token.handPiece.PlayerOwner.PlayerColor);
            UpdateHandPiece(ref token.handPiece);
            GotoTurnEndStep();
        }

        private void DropPiece(ref PieceEV pieceToDrop, ref TileEV destinationTile, PlayerColor playerOwner)
        {
            Vector2 location = destinationTile.Location.Location;

            pieceSetService.SetPieceLocationAndTier(pieceToDrop, location, 1, entitiesDB);
            pieceSetService.SetPiecePlayerOwner(pieceToDrop, playerOwner, entitiesDB);
            pieceToDrop.MovePiece.NewLocation = location;
            pieceToDrop.Visibility.IsVisible.value = true;
            pieceToDrop.ChangeColorTrigger.PlayChangeColor = true;
        }

        private void UpdateHandPiece(ref HandPieceEV handPiece)
        {
            handService.DecrementHandPiece(ref handPiece);
        }

        private void GotoTurnEndStep()
        {
            var turnEndToken = new TurnEndStepState();
            dropSequence.Next(this, ref turnEndToken);
        }
    }
}
