using Data.Enum;
using Data.Step;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Board.Tile;
using Service.Highlight;
using Service.Piece;
using Service.Turn;
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

            // TODO Use Linq Where() filtering
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
            PieceEV piece = PieceService.FindPieceEV(token.pieceEntityId, entitiesDB);
            int pieceIdtoken = token.pieceEntityId;
            HighlightState newHighlightState = HighlightService.CalcClickHighlightState(piece.playerOwner.PlayerColor);
            TurnEV currentTurn = TurnService.GetCurrentTurnEV(entitiesDB);
            bool doesPieceBelongToTurnPlayer = currentTurn.TurnPlayer.PlayerColor == piece.playerOwner.PlayerColor;

            foreach (TileEV tileEV in tilesToChange)
            {
                entitiesDB.ExecuteOnEntity(
                    tileEV.ID,
                    (ref TileEV tileToChange) =>
                    {
                        tileToChange.highlight.IsHighlighted = isClicked;

                        if (doesPieceBelongToTurnPlayer)
                        {
                            tileToChange.tile.PieceRefEntityId = isClicked ? (int?)pieceIdtoken : null;
                        }

                        if (isClicked)
                        {
                            tileToChange.highlight.CurrentColorStates.Add(newHighlightState);
                        }
                        else
                        {
                            tileToChange.highlight.CurrentColorStates.Remove(newHighlightState);
                        }
                    });

                tileEV.changeColorComponent.PlayChangeColor = true;
            }
        }
    }
}