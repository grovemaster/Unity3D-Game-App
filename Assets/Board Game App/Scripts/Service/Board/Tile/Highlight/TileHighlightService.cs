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
    public class TileHighlightService
    {
        private TileService tileService = new TileService();

        public void DeHighlightOtherTeamTilePieces(List<PieceEV> alteredPieces, PlayerColor pieceTeam, IEntitiesDB entitiesDB)
        {
            HighlightState highlightStateToRemove = HighlightService.CalcClickHighlightState(pieceTeam);

            // TODO Remove team highlights based on Team Color, not piece ref id
            List<TileEV> tiles = tileService.FindAllTileEVs(entitiesDB)
                .Where(tile => tile.Highlight.IsHighlighted
                && tile.Highlight.CurrentColorStates.Contains(highlightStateToRemove)
                ).ToList();

            foreach (TileEV tile in tiles)
            {
                entitiesDB.ExecuteOnEntity(
                    tile.ID,
                    (ref TileEV tileToChange) =>
                    {
                        tileToChange.Highlight.CurrentColorStates.Remove(highlightStateToRemove);
                        tileToChange.Tile.PieceRefEntityId = null;

                        if (!tileToChange.Highlight.CurrentColorStates.Any())
                        {
                            tileToChange.Highlight.IsHighlighted = false;
                        }
                    });

                tile.ChangeColorTrigger.PlayChangeColor = true;
            }
        }
    }
}
