using Data.Enums;
using Data.Step.Board;
using Data.Step.Drop;
using Data.Step.Piece.Ability.Substitution;
using Data.Step.Piece.Ability.TierExchange;
using Data.Step.Piece.Click;
using Data.Step.Piece.Move;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Scripts.Data.Board;
using Service.Board;
using Service.Turn;
using Svelto.ECS;
using System;

namespace ECS.Engine.Board
{
    class BoardPressEngine : IStep<BoardPressStepState>, IQueryingEntitiesEngine
    {
        private BoardPressService boardPressService = new BoardPressService();
        private PieceTileService pieceTileService = new PieceTileService();
        private TurnService turnService = new TurnService();

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

            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            BoardPressStateInfo stateInfo = pieceTileService.FindBoardPressStateInfo(entitiesDB, ref token);
            bool substitutionPossible = pieceTileService.IsSubstitutionPossible(stateInfo, currentTurn, entitiesDB);
            bool tierExchangePossible = pieceTileService.IsTierExchangePossible(stateInfo, currentTurn, entitiesDB);

            BoardPress action = boardPressService.DecideAction(
                stateInfo,
                substitutionPossible,
                tierExchangePossible,
                currentTurn);
            ExecuteNextAction(action, stateInfo);
        }

        /// <exception cref="InvalidOperationException">Both token member variables are null/zero.</exception>
        private void ConstraintCheck(ref BoardPressStepState token)
        {
            if (token.PieceEntityId.GetValueOrDefault() == 0
                && token.TileEntityId.GetValueOrDefault() == 0)
            {
                throw new InvalidOperationException("BoardPressEngine: Both piece and tile id are null");
            }
        }

        private void ExecuteNextAction(BoardPress action, BoardPressStateInfo stateInfo)
        {
            switch(action)
            {
                case BoardPress.CLICK_HIGHLIGHT:
                    NextActionHighlight(stateInfo.piece.Value);
                    break;
                case BoardPress.MOVE_PIECE:
                    NextActionMovePiece(stateInfo.piece.Value, stateInfo.tile.Value);
                    break;
                case BoardPress.DROP:
                    NextActionDropPiece(stateInfo.handPiece.Value, stateInfo.tile.Value);
                    break;
                case BoardPress.SUBSTITUTION:
                    NextActionSubstitution(stateInfo.piece.Value, stateInfo.tile.Value);
                    break;
                case BoardPress.TIER_1_3_EXCHANGE:
                    NextActionTierExchange(stateInfo.tile.Value);
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
            var clickPieceStepState = new ClickPieceStepState
            {
                ClickedPiece = pieceEV
            };

            boardPressSequence.Next(this, ref clickPieceStepState, (int)BoardPress.CLICK_HIGHLIGHT);
        }

        private void NextActionMovePiece(PieceEV pieceEV, TileEV tileEV)
        {
            var movePieceInfo = new MovePieceStepState
            {
                PieceToMove = pieceEV,
                DestinationTile = tileEV
            };

            boardPressSequence.Next(this, ref movePieceInfo, (int)BoardPress.MOVE_PIECE);
        }

        private void NextActionDropPiece(HandPieceEV handPiece, TileEV destinationTile)
        {
            var dropInfo = new DropPrepStepState
            {
                HandPiece = handPiece,
                DestinationTile = destinationTile
            };

            boardPressSequence.Next(this, ref dropInfo, (int)BoardPress.DROP);
        }

        private void NextActionSubstitution(PieceEV piece, TileEV tile)
        {
            var substitutionToken = new SubstitutionStepState
            {
                SubstitutionPiece = piece,
                TileReferenceEV = tile
            };

            boardPressSequence.Next(this, ref substitutionToken, (int)BoardPress.SUBSTITUTION);
        }

        private void NextActionTierExchange(TileEV tile)
        {
            var tierExchangeToken = new TierExchangeStepState
            {
                ReferenceTile = tile
            };

            boardPressSequence.Next(this, ref tierExchangeToken, (int)BoardPress.TIER_1_3_EXCHANGE);
        }
    }
}
