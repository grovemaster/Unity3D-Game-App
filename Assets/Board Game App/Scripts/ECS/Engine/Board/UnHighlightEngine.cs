using Data.Enum;
using Data.Step.Piece.Move;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Board.Tile;
using Service.Piece;
using Svelto.ECS;

namespace ECS.Engine.Board
{
    class UnHighlightEngine : IStep<MovePieceStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref MovePieceStepState token, int condition)
        {
            UnHighlightPieces();
            UnHighlightTiles();
        }

        private void UnHighlightPieces()
        {
            PieceEV[] pieceEVs = PieceService.FindAllPieceEVs(entitiesDB);

            for (int i = 0; i < pieceEVs.Length; ++i)
            {
                if (pieceEVs[i].highlight.IsHighlighted)
                {
                    entitiesDB.ExecuteOnEntity(
                        pieceEVs[i].ID,
                        (ref PieceEV pieceToChange) => { pieceToChange.highlight.IsHighlighted = false; });
                    pieceEVs[i].highlight.CurrentColor.value = HighlightState.DEFAULT;
                }
            }
        }

        private void UnHighlightTiles()
        {
            TileEV[] tileEVs = TileService.FindAllTileEVs(entitiesDB);

            for (int i = 0; i < tileEVs.Length; ++i)
            {
                if (tileEVs[i].highlight.IsHighlighted)
                {
                    entitiesDB.ExecuteOnEntity(
                        tileEVs[i].ID,
                        (ref TileEV tileToChange) => { tileToChange.highlight.IsHighlighted = false; });
                    tileEVs[i].highlight.CurrentColor.value = HighlightState.DEFAULT;
                }
            }
        }
    }
}
