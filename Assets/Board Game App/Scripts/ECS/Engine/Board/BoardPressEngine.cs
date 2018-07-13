using Data.Enum;
using Data.Step;
using Data.Step.Board;
using Data.Step.Piece.Move;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Board.Tile;
using Service.Piece;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ECS.Engine.Board
{
    class BoardPressEngine : IStep<BoardPressState>, IQueryingEntitiesEngine
    {
        private readonly ISequencer boardPressSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public BoardPressEngine(ISequencer boardPressSequence)
        {
            this.boardPressSequence = boardPressSequence;
        }

        public void Ready()
        { }

        public void Step(ref BoardPressState token, int condition)
        {
            ConstraintCheck(ref token);

            PieceEV? pieceEV;
            TileEV? tileEV;
            FindPieceTileEV(ref token, out pieceEV, out tileEV);

            BoardPress action = DecideAction(pieceEV, tileEV);
            ExecuteNextAction(action, pieceEV, tileEV);
        }

        private void ConstraintCheck(ref BoardPressState token)
        {
            int pieceEntityId = token.pieceEntityId ?? 0;
            int tileEntityId = token.tileEntityId ?? 0;

            if (pieceEntityId == 0 && tileEntityId == 0)
            {
                throw new InvalidOperationException("BoardPressEngine: Both piece and tile id are null");
            }
        }

        private void FindPieceTileEV(ref BoardPressState token, out PieceEV? pieceEV, out TileEV? tileEV)
        {
            pieceEV = FindPieceEVById(token.pieceEntityId);
            tileEV = FindTileEVById(token.tileEntityId);

            if (pieceEV == null
                && tileEV != null
                && ((TileEV)tileEV).tile.PieceRefEntityId != null
                && ((TileEV)tileEV).tile.PieceRefEntityId != 0) // Find by tile information
            {
                pieceEV = FindPieceEVById((int)((TileEV)tileEV).tile.PieceRefEntityId);
            }

            if (tileEV == null) // Find by piece information
            {
                var location = ((PieceEV)pieceEV).location;
                tileEV = TileService.FindTileEV(
                    new Vector3(location.Location.x, location.Location.y, 0),
                    entitiesDB);
            }

            if (tileEV != null && pieceEV == null)
            {
                pieceEV = FindPieceByLocation(((TileEV)tileEV).location.Location);
            }
        }

        private PieceEV? FindPieceEVById(int? entityId)
        {
            PieceEV? returnValue = null;

            if (entityId != null && entityId != 0)
            {
                returnValue = PieceService.FindPieceEV((int)entityId, entitiesDB);
            }

            return returnValue;
        }

        private TileEV? FindTileEVById(int? entityId)
        {
            TileEV? returnValue = null;

            if (entityId != null && entityId != 0)
            {
                returnValue = TileService.FindTileEV((int)entityId, entitiesDB);
            }

            return returnValue;
        }

        // TODO This will become a list once towers are enabled
        private PieceEV? FindPieceByLocation(Vector3 location)
        {
            PieceEV? returnValue = null;

            int count;
            PieceEV[] pieces = PieceService.FindAllPieceEVs(entitiesDB, out count);

            for (int i = 0; i < count; ++i)
            {
                if (pieces[i].location.Location == location)
                {
                    returnValue = pieces[i];
                    break;
                }
            }

            return returnValue;
        }

        private List<Vector3> FindAffectedTiles(int entityId)
        {
            PieceEV pieceEV = PieceService.FindPieceEV(entityId, entitiesDB);

            return PieceService.CreateIPieceData(pieceEV.piece.PieceType).Tiers()[0].Single()
                .Select(x => new Vector3(x.x, x.y, 0)).ToList(); // Change z-value from 1 to 0
        }

        private BoardPress DecideAction(PieceEV? pieceEV, TileEV? tileEVParam)
        {
            BoardPress returnValue = BoardPress.NOTHING;
            TileEV tileEV = (TileEV)tileEVParam;
            int tilePieceId = tileEV.tile.PieceRefEntityId ?? 0;

            if (!tileEV.highlight.IsHighlighted.value && pieceEV != null)
            {
                returnValue = BoardPress.CLICK_HIGHLIGHT;
            }
            else if (tileEV.highlight.IsHighlighted.value && pieceEV != null && tilePieceId != 0)
            {
                returnValue = BoardPress.MOVE_PIECE;
            }

            return returnValue;
        }

        private void ExecuteNextAction(BoardPress action, PieceEV? pieceEV, TileEV? tileEV)
        {
            switch(action)
            {
                case BoardPress.CLICK_HIGHLIGHT:
                    NextActionHighlight((PieceEV)pieceEV);
                    break;
                case BoardPress.MOVE_PIECE:
                    NextActionMovePiece((PieceEV)pieceEV, (TileEV)tileEV);
                    break;
                case BoardPress.NOTHING:
                    break;
                default:
                    throw new InvalidOperationException("Invalid or unsupported BoardPress state");
            }
        }

        private void NextActionHighlight(PieceEV pieceEV)
        {
            var pressState = new PressState
            {
                pieceEntityId = pieceEV.ID.entityID,
                piecePressState = pieceEV.highlight.IsHighlighted.value ? PiecePressState.CLICKED : PiecePressState.UNCLICKED,
                affectedTiles = FindAffectedTiles(pieceEV.ID.entityID)
            };

            boardPressSequence.Next(this, ref pressState, (int)BoardPress.CLICK_HIGHLIGHT);
        }

        private void NextActionMovePiece(PieceEV pieceEV, TileEV tileEV)
        {
            var movePieceInfo = new MovePieceInfo
            {
                pieceToMove = pieceEV,
                destinationTile = tileEV
            };

            boardPressSequence.Next(this, ref movePieceInfo, (int)BoardPress.MOVE_PIECE);
        }
    }
}
