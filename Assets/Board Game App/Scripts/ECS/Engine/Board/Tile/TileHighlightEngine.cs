using Data.Enum;
using Data.Step;
using ECS.EntityView.Board.Tile;
using Service.Board.Tile;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Board.Tile
{
    public class TileHighlightEngine : IStep<PressStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

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
            var entityViews = TileService.FindAllTileEVs(entitiesDB);

            for (int i = 0; i < entityViews.Length; ++i)
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
            bool isClicked = token.piecePressState == PiecePressState.CLICKED;

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