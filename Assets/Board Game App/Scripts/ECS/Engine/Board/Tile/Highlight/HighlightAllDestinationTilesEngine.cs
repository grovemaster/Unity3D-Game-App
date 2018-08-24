using Data.Enum;
using Data.Enum.Player;
using Data.Step.Turn;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using ECS.EntityView.Turn;
using Service.Board;
using Service.Board.Tile;
using Service.Highlight;
using Service.Piece.Find;
using Service.Turn;
using Svelto.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ECS.Engine.Board.Tile.Highlight
{
    public class HighlightAllDestinationTilesEngine : IStep<TurnStartStepState>, IQueryingEntitiesEngine
    {
        private DestinationTileService destinationTileService = new DestinationTileService();
        private PieceFindService pieceFindService = new PieceFindService();
        private TileService tileService = new TileService();
        private TurnService turnService = new TurnService();

        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready()
        { }

        public void Step(ref TurnStartStepState token, int condition)
        {
            TurnEV turnEV = turnService.GetCurrentTurnEV(entitiesDB);
            TileEV[] tiles = tileService.FindAllTileEVs(entitiesDB);

            FindAndHighlightTeamTiles(PlayerColor.BLACK, tiles, PlayerColor.BLACK == turnEV.TurnPlayer.PlayerColor);
            FindAndHighlightTeamTiles(PlayerColor.WHITE, tiles, PlayerColor.WHITE == turnEV.TurnPlayer.PlayerColor);

            PlayColorAllTiles(tiles);
        }

        private void FindAndHighlightTeamTiles(PlayerColor teamColor, TileEV[] tiles, bool excludeCheckViolations)
        {
            PieceEV[] teamPieces = pieceFindService.FindPiecesByTeam(teamColor, entitiesDB);

            HashSet<Vector2> destinationLocations =
                destinationTileService.CalcDestinationTileLocations(teamPieces, excludeCheckViolations, entitiesDB);

            HighlightTiles(tiles, destinationLocations, HighlightService.CalcRangeHighlightState(teamColor));
        }

        private void HighlightTiles(
            TileEV[] tiles, HashSet<Vector2> locations, HighlightState highlightStateToAdd)
        {
            foreach (TileEV tile in tiles)
            {
                if (locations.Contains(tile.Location.Location))
                {
                    entitiesDB.ExecuteOnEntity(
                        tile.ID,
                        (ref TileEV tileToChange) =>
                        {
                            tileToChange.Highlight.CurrentColorStates.Add(highlightStateToAdd);
                        });
                }
            }
        }

        private void PlayColorAllTiles(TileEV[] tiles)
        {
            foreach(TileEV tile in tiles)
            {
                tile.ChangeColorTrigger.PlayChangeColor = true;
            }
        }
    }
}
