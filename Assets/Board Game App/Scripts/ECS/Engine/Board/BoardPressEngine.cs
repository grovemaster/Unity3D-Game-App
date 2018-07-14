using Data.Enum;
using Data.Step;
using Data.Step.Board;
using Data.Step.Piece.Move;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Board;
using Service.Turn;
using Svelto.ECS;
using System;

namespace ECS.Engine.Board
{
    class BoardPressEngine : IStep<BoardPressStepState>, IQueryingEntitiesEngine
    {
        private readonly ISequencer boardPressSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public BoardPressEngine(ISequencer boardPressSequence)
        {
            this.boardPressSequence = boardPressSequence;
        }

        public void Ready()
        { }

        public void Step(ref BoardPressStepState token, int condition)
        {
            ConstraintCheck(ref token);

            PieceEV? pieceEV;
            TileEV? tileEV;
            PieceTileService.FindPieceTileEV(entitiesDB, ref token, out pieceEV, out tileEV);

            TurnEV currentTurn = TurnService.GetCurrentTurnEV(entitiesDB);

            BoardPress action = BoardPressService.DecideAction(pieceEV, tileEV, currentTurn);
            ExecuteNextAction(action, pieceEV, tileEV);
        }

        /// <exception cref="InvalidOperationException">Both token member variables are null/zero.</exception>
        private void ConstraintCheck(ref BoardPressStepState token)
        {
            int pieceEntityId = token.pieceEntityId ?? 0;
            int tileEntityId = token.tileEntityId ?? 0;

            if (pieceEntityId == 0 && tileEntityId == 0)
            {
                throw new InvalidOperationException("BoardPressEngine: Both piece and tile id are null");
            }
        }

        private void ExecuteNextAction(BoardPress action, PieceEV? pieceEV, TileEV? tileEV)
        {
            switch(action)
            {
                case BoardPress.CLICK_HIGHLIGHT:
                    NextActionHighlight(pieceEV.Value);
                    break;
                case BoardPress.MOVE_PIECE:
                    NextActionMovePiece(pieceEV.Value, tileEV.Value);
                    break;
                case BoardPress.NOTHING:
                    break;
                default:
                    throw new InvalidOperationException("Invalid or unsupported BoardPress state");
            }
        }

        private void NextActionHighlight(PieceEV pieceEV)
        {
            // Give desired state, up to later engines to make changes accordingly
            var pressState = new PressStepState
            {
                pieceEntityId = pieceEV.ID.entityID,
                piecePressState = pieceEV.highlight.IsHighlighted ? PiecePressState.UNCLICKED : PiecePressState.CLICKED,
                affectedTiles = DestinationTileService.FindDestinationTileLocations(pieceEV.ID.entityID, entitiesDB)
            };

            boardPressSequence.Next(this, ref pressState, (int)BoardPress.CLICK_HIGHLIGHT);
        }

        private void NextActionMovePiece(PieceEV pieceEV, TileEV tileEV)
        {
            var movePieceInfo = new MovePieceStepState
            {
                pieceToMove = pieceEV,
                destinationTile = tileEV
            };

            boardPressSequence.Next(this, ref movePieceInfo, (int)BoardPress.MOVE_PIECE);
        }
    }
}
