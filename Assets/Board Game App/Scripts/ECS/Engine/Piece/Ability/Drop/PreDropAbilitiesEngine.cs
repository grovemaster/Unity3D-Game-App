using System;
using Data.Enum;
using Data.Enum.AB;
using Data.Enum.Piece.Drop;
using Data.Enum.Piece.Side;
using Data.Step.Drop;
using Service.Piece.Factory;
using Service.Piece.Find;
using Svelto.ECS;

namespace ECS.Engine.Piece.Ability.Drop
{
    class PreDropAbilitiesEngine : IStep<DropPrepStepState>, IQueryingEntitiesEngine
    {
        private PieceFactory pieceFactory = new PieceFactory();
        private PieceFindService pieceFindService = new PieceFindService();

        private readonly ISequencer dropSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public PreDropAbilitiesEngine(ISequencer dropSequence)
        {
            this.dropSequence = dropSequence;
        }

        public void Ready()
        { }

        public void Step(ref DropPrepStepState token, int condition)
        {
            bool isFrontValid = IsValidDrop(ref token);
            bool isBackValid = true;

            if (isFrontValid && isBackValid)
            {
                NextActionDropModal(ref token);
            }
            else if (isFrontValid ^ isBackValid)
            {
                NextActionDrop(ref token, isFrontValid ? PieceSide.FRONT : PieceSide.BACK);
            }
        }

        private bool IsValidDrop(ref DropPrepStepState token)
        {
            return !HasDoublePawnDrop(token.HandPiece.HandPiece.PieceType) || IsValidDropTile(ref token);
        }

        private bool HasDoublePawnDrop(PieceType pieceType)
        {
            DropAbility? dropAbility = pieceFactory.CreateIPieceData(pieceType).Abilities.Drop;
            return dropAbility.HasValue && dropAbility.Value == DropAbility.DOUBLE_PAWN_DROP;
        }

        private bool IsValidDropTile(ref DropPrepStepState token)
        {
            return pieceFindService.FindPiecesByTypeAndFile(
                token.HandPiece.HandPiece.PieceType,
                token.DestinationTile.Location.Location.x,
                entitiesDB
                ).Count == 0;
        }

        private void NextActionDropModal(ref DropPrepStepState token)
        {
            dropSequence.Next(this, ref token, (int)StepAB.A);
        }

        private void NextActionDrop(ref DropPrepStepState token, PieceSide pieceSide)
        {
            var dropToken = new DropStepState
            {
                DestinationTile = token.DestinationTile,
                HandPiece = token.HandPiece,
                Side = pieceSide
            };

            dropSequence.Next(this, ref dropToken, (int)StepAB.B);
        }
    }
}
