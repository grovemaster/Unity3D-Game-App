﻿using Data.Step.Piece.Move;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;
using Service.Board.Tile;
using Service.Hand;
using Service.Piece;
using Svelto.ECS;
using System.Collections.Generic;
using System.Linq;

namespace ECS.Engine.Board
{
    class UnHighlightEngine : IStep<MovePieceStepState>, IQueryingEntitiesEngine
    {
        private HandService handService = new HandService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref MovePieceStepState token, int condition)
        {
            UnHighlightPieces();
            UnHighlightTiles();
            UnHighlightHandPieces();
        }

        private void UnHighlightPieces()
        {
            PieceEV[] pieceEVs = PieceService.FindAllBoardPieces(entitiesDB);

            for (int i = 0; i < pieceEVs.Length; ++i)
            {
                if (pieceEVs[i].highlight.IsHighlighted)
                {
                    entitiesDB.ExecuteOnEntity(
                        pieceEVs[i].ID,
                        (ref PieceEV pieceToChange) =>
                        {
                            pieceToChange.highlight.IsHighlighted = false;
                            pieceToChange.highlight.CurrentColorStates.Clear();
                        });

                    pieceEVs[i].changeColor.PlayChangeColor = true;
                }
            }
        }

        private void UnHighlightTiles()
        {
            TileEV[] tileEVs = TileService.FindAllTileEVs(entitiesDB);

            for (int i = 0; i < tileEVs.Length; ++i)
            {
                // TODO Sloppy coding; if gated with an if statement, captured piece's range highlight remains.
                entitiesDB.ExecuteOnEntity(
                    tileEVs[i].ID,
                    (ref TileEV tileToChange) =>
                    {
                        tileToChange.highlight.IsHighlighted = false;
                        tileToChange.highlight.CurrentColorStates.Clear();
                    });

                tileEVs[i].changeColorComponent.PlayChangeColor = true;
            }
        }

        private void UnHighlightHandPieces()
        {
            handService.UnHighlightAllHandPieces(entitiesDB);
        }
    }
}
