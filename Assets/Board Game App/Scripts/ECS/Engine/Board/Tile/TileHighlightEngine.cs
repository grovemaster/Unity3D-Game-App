using Data.Enum;
using Data.Step;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Board.Tile;
using Service.Highlight;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Board.Tile
{
    public class TileHighlightEngine : IStep<PressStepState>, IQueryingEntitiesEngine
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private TileService tileService = new TileService();
        private TurnService turnService = new TurnService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref PressStepState token, int condition)
        {
            List<TileEV> tilesToChange = FindTilesToChange(token.affectedTiles);
            ChangeTileColor(tilesToChange, ref token);
        }

        private List<TileEV> FindTilesToChange(List<Vector2> affectedTiles)
        {
            List<TileEV> returnValue = new List<TileEV>();

            // TODO Cache all Tiles, since they will not change
            var entityViews = tileService.FindAllTileEVs(entitiesDB);

            // TODO Use Linq Where() filtering
            for (int i = 0; i < entityViews.Length; ++i)
            {
                if (affectedTiles.Contains(entityViews[i].Location.Location))
                {
                    returnValue.Add(entityViews[i]);
                }
            }

            return returnValue;
        }

        private void ChangeTileColor(List<TileEV> tilesToChange, ref PressStepState token)
        {
            bool isClicked = token.piecePressState == PiecePressState.CLICKED;
            PieceEV piece = pieceFindService.FindPieceEV(token.pieceEntityId, entitiesDB);
            int pieceIdtoken = token.pieceEntityId;
            HighlightState newHighlightState = HighlightService.CalcClickHighlightState(piece.PlayerOwner.PlayerColor);
            TurnEV currentTurn = turnService.GetCurrentTurnEV(entitiesDB);
            bool doesPieceBelongToTurnPlayer = currentTurn.TurnPlayer.PlayerColor == piece.PlayerOwner.PlayerColor;

            foreach (TileEV tileEV in tilesToChange)
            {
                entitiesDB.ExecuteOnEntity(
                    tileEV.ID,
                    (ref TileEV tileToChange) =>
                    {
                        tileToChange.Highlight.IsHighlighted = isClicked;

                        if (doesPieceBelongToTurnPlayer)
                        {
                            tileToChange.Tile.PieceRefEntityId = isClicked ? (int?)pieceIdtoken : null;
                        }

                        if (isClicked)
                        {
                            tileToChange.Highlight.CurrentColorStates.Add(newHighlightState);
                        }
                        else
                        {
                            tileToChange.Highlight.CurrentColorStates.Remove(newHighlightState);
                        }
                    });

                tileEV.ChangeColorTrigger.PlayChangeColor = true;
            }
        }
    }
}