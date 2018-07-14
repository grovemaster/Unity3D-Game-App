﻿using Data.Enum;
using Data.Step;
using ECS.EntityView.Board.Tile;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Board.Tile
{
    public class TileHighlightEngine : IStep<PressStepState>, IQueryingEntitiesEngine
    {
        private readonly ISequencer highlightSequence;

        public IEntitiesDB entitiesDB { private get; set; }

        public TileHighlightEngine(ISequencer highlightSequence)
        {
            this.highlightSequence = highlightSequence;
        }

        public void Ready()
        { }

        public void Step(ref PressStepState token, int condition)
        {
            List<TileEV> tilesToChange = FindTilesToChange(token.affectedTiles);
            ChangeTileColor(tilesToChange, ref token);
        }

        private List<TileEV> FindTilesToChange(List<Vector3> affectedTiles)
        {
            List<TileEV> returnValue = new List<TileEV>();

            // TODO Cache all Tiles, since they will not change
            int count;
            var entityViews = entitiesDB.QueryEntities<TileEV>(out count);

            for (int i = 0; i < count; ++i)
            {
                if (affectedTiles.Contains(entityViews[i].location.Location))
                {
                    returnValue.Add(entityViews[i]);
                }
            }

            return returnValue;
        }

        private void ChangeTileColor(List<TileEV> tilesToChange, ref PressStepState token)
        {
            bool isClicked = token.piecePressState.Equals(PiecePressState.CLICKED);

            foreach (TileEV tileEV in tilesToChange)
            {
                entitiesDB.ExecuteOnEntity(
                    tileEV.ID,
                    (ref TileEV pieceToChange) => { pieceToChange.highlight.IsHighlighted = isClicked ? true : false; });
                tileEV.highlight.CurrentColor.value = isClicked
                    ? HighlightState.CLICKED : HighlightState.DEFAULT;

                int? pieceIdtoken = null;
                if (isClicked)
                {
                    pieceIdtoken = token.pieceEntityId;
                }

                entitiesDB.ExecuteOnEntity(tileEV.ID,
                    (ref TileEV tileEVToChange) => { tileEVToChange.tile.PieceRefEntityId = pieceIdtoken; } );
            }
        }
    }
}