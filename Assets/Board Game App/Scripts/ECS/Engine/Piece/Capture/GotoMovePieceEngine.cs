﻿using Data.Step.Piece.Capture;
using Data.Step.Piece.Move;
using Svelto.ECS;

namespace ECS.Engine.Piece.Capture
{
    class GotoMovePieceEngine : IStep<CapturePieceStepState>, IQueryingEntitiesEngine
    {
        private Sequencer gotoMovePiece;

        public IEntitiesDB entitiesDB { private get; set; }

        public GotoMovePieceEngine(Sequencer gotoMovePiece)
        {
            this.gotoMovePiece = gotoMovePiece;
        }

        public void Ready()
        { }

        public void Step(ref CapturePieceStepState token, int condition)
        {
            var movePieceInfo = new MovePieceStepState
            {
                pieceToMove = token.pieceToMove,
                pieceToCapture = token.pieceToCapture,
                destinationTile = token.destinationTile
            };

            gotoMovePiece.Next(this, ref movePieceInfo);
        }
    }
}
