﻿using Data.Enum;
using Data.Enum.Player;
using Data.Step;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Board.Tile;
using Service.Piece;
using Svelto.ECS;
using System.Collections.Generic;
using System.Linq;

namespace ECS.Engine.Piece
{
    class DeHighlightTeamPiecesEngine : IStep<PressStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref PressStepState token, int condition)
        {
            // Purpose of this engine is to ensure only a single piece per team is highlighted at any given time
            // If the goal is to UN-highlight a piece, there is no need to check for other highlighted pieces
            if (token.piecePressState == PiecePressState.UNCLICKED)
            {
                return;
            }

            PieceEV piece = PieceService.FindPieceEV(token.pieceEntityId, entitiesDB);

            List<PieceEV> alteredPieces = DeHighlightOtherPlayerPieces(
                token.pieceEntityId, piece.playerOwner.PlayerColor);

            if (alteredPieces.Count > 0)
            {
                DeHighlightOtherTeamTilePieces(alteredPieces, piece.playerOwner.PlayerColor);
            }
        }

        private List<PieceEV> DeHighlightOtherPlayerPieces(int pieceEntityId, PlayerColor pieceTeam)
        {
            List<PieceEV> pieces = PieceService.FindPiecesByTeam(pieceTeam, entitiesDB)
                .Where(piece => piece.ID.entityID != pieceEntityId && piece.highlight.IsHighlighted).ToList();

            foreach (PieceEV piece in pieces)
            {
                entitiesDB.ExecuteOnEntity(
                    piece.ID,
                    (ref PieceEV pieceToChange) => { pieceToChange.highlight.IsHighlighted = false; });
                piece.highlight.CurrentColor.value = HighlightState.DEFAULT;
            }

            return pieces;
        }

        private void DeHighlightOtherTeamTilePieces(List<PieceEV> alteredPieces, PlayerColor pieceTeam)
        {
            List<TileEV> tiles = TileService.FindAllTileEVs(entitiesDB)
                .Where(tile => tile.highlight.IsHighlighted
                && alteredPieces.FindIndex(piece => piece.ID.entityID == tile.tile.PieceRefEntityId.GetValueOrDefault()) >= 0
                ).ToList();

            foreach (TileEV tile in tiles)
            {
                entitiesDB.ExecuteOnEntity(
                    tile.ID,
                    (ref TileEV tileToChange) => { tileToChange.highlight.IsHighlighted = false; });
                tile.highlight.CurrentColor.value = HighlightState.DEFAULT;
            }
        }
    }
}