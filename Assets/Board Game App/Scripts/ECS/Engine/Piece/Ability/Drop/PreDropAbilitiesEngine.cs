using Data.Enum;
using Data.Enum.Piece.Drop;
using Data.Step.Drop;
using Service.Piece.Factory;
using Service.Piece.Find;
using Svelto.ECS;

namespace ECS.Engine.Piece.Ability.Drop
{
    class PreDropAbilitiesEngine : IStep<DropStepState>, IQueryingEntitiesEngine
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

        public void Step(ref DropStepState token, int condition)
        {
            if (IsValidDrop(ref token))
            {
                NextActionDrop(ref token);
            }
        }

        private bool IsValidDrop(ref DropStepState token)
        {
            return !HasDoublePawnDrop(token.handPiece.HandPiece.PieceType) || IsValidDropTile(ref token);
        }

        private bool HasDoublePawnDrop(PieceType pieceType)
        {
            DropAbility? dropAbility = pieceFactory.CreateIPieceData(pieceType).Abilities.Drop;
            return dropAbility.HasValue && dropAbility.Value == DropAbility.DOUBLE_PAWN_DROP;
        }

        private bool IsValidDropTile(ref DropStepState token)
        {
            return pieceFindService.FindPiecesByTypeAndFile(
                token.handPiece.HandPiece.PieceType,
                token.destinationTile.Location.Location.x,
                entitiesDB
                ).Count == 0;
        }

        private void NextActionDrop(ref DropStepState token)
        {
            dropSequence.Next(this, ref token);
        }
    }
}
