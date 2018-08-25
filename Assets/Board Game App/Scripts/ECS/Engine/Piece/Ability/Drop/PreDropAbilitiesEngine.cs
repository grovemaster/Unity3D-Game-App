using Data.Enum;
using Data.Enum.Piece.Drop;
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
            if (IsValidDrop(ref token))
            {
                NextActionDrop(ref token);
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

        private void NextActionDrop(ref DropPrepStepState token)
        {
            dropSequence.Next(this, ref token);
        }
    }
}
