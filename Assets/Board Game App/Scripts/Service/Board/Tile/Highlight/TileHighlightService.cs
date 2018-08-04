using Data.Enum;
using Data.Enum.Player;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Highlight;
using Svelto.ECS;
using System.Collections.Generic;
using System.Linq;

namespace Service.Board.Tile.Highlight
{
    class TileHighlightService
    {
        public void DeHighlightOtherTeamTilePieces(List<PieceEV> alteredPieces, PlayerColor pieceTeam, IEntitiesDB entitiesDB)
        {
            HighlightState highlightStateToRemove = HighlightService.CalcClickHighlightState(pieceTeam);

            // TODO Remove team highlights based on Team Color, not piece ref id
            List<TileEV> tiles = TileService.FindAllTileEVs(entitiesDB)
                .Where(tile => tile.highlight.IsHighlighted
                && tile.highlight.CurrentColorStates.Contains(highlightStateToRemove)
                ).ToList();

            foreach (TileEV tile in tiles)
            {
                entitiesDB.ExecuteOnEntity(
                    tile.ID,
                    (ref TileEV tileToChange) =>
                    {
                        tileToChange.highlight.CurrentColorStates.Remove(highlightStateToRemove);
                        tileToChange.tile.PieceRefEntityId = null;

                        if (!tileToChange.highlight.CurrentColorStates.Any())
                        {
                            tileToChange.highlight.IsHighlighted = false;
                        }
                    });

                tile.changeColorComponent.PlayChangeColor = true;
            }
        }
    }
}
