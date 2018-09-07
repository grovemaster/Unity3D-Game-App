using Data.Enums.AB;
using Data.Enums.Piece.Side;
using Data.Step.Drop;
using ECS.EntityView.Piece;
using Service.Drop;
using Service.Piece.Find;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Engine.Piece.Ability.Drop
{
    class PreDropAbilitiesEngine : IStep<DropPrepStepState>, IQueryingEntitiesEngine
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private PreDropService preDropService = new PreDropService();

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
            List<PieceEV> piecesAtLocation = pieceFindService.FindPiecesByLocation(token.DestinationTile.Location.Location, entitiesDB);
            bool isFrontValid = preDropService.IsValidFrontDrop(ref token.HandPiece, token.DestinationTile.Location.Location, piecesAtLocation, entitiesDB);
            bool isBackValid = preDropService.IsValidBackDrop(ref token.HandPiece, token.DestinationTile.Location.Location, piecesAtLocation, entitiesDB);

            if (isFrontValid && isBackValid)
            {
                NextActionDropModal(ref token);
            }
            else if (isFrontValid ^ isBackValid)
            {
                NextActionDrop(ref token, isFrontValid ? PieceSide.FRONT : PieceSide.BACK);
            }
        }

        #region NextAction
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
        #endregion
    }
}
