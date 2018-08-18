using Data.Enum;
using Data.Enum.Player;
using Data.Step.Turn;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Board;
using Service.Board.Tile;
using Service.Highlight;
using Service.Piece.Find;
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

        public IEntitiesDB entitiesDB { set; private get; }

        public void Ready()
        { }

        public void Step(ref TurnStartStepState token, int condition)
        {
            TileEV[] tiles = tileService.FindAllTileEVs(entitiesDB);

            FindAndHighlightTeamTiles(PlayerColor.BLACK, tiles);
            FindAndHighlightTeamTiles(PlayerColor.WHITE, tiles);

            PlayColorAllTiles(tiles);
        }

        private void FindAndHighlightTeamTiles(PlayerColor teamColor, TileEV[] tiles)
        {
            PieceEV[] teamPieces = pieceFindService.FindPiecesByTeam(teamColor, entitiesDB);

            HashSet<Vector2> destinationLocations =
                destinationTileService.CalcDestinationTileLocations(teamPieces, entitiesDB);

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
