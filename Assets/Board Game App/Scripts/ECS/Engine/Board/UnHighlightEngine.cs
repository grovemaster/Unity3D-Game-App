using Data.Step.Piece.Move;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Board.Tile;
using Service.Piece;
using Svelto.ECS;

namespace ECS.Engine.Board
{
    class UnHighlightEngine : IStep<MovePieceInfo>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref MovePieceInfo token, int condition)
        {
            UnHighlightPieces();
            UnHighlightTiles();
        }

        private void UnHighlightPieces()
        {
            int count;
            PieceEV[] pieceEVs = PieceService.FindAllPieceEVs(entitiesDB, out count);

            for (int i = 0; i < count; ++i)
            {
                // TODO If setting to same value does not trigger notify, then remove if condition
                if (pieceEVs[i].highlight.IsHighlighted.value)
                {
                    pieceEVs[i].highlight.IsHighlighted.value = false;
                }
            }
        }

        private void UnHighlightTiles()
        {
            int count;
            TileEV[] tileEVs = TileService.FindAllTileEVs(entitiesDB, out count);

            for (int i = 0; i < count; ++i)
            {
                // TODO If setting to same value does not trigger notify, then remove if condition
                if (tileEVs[i].highlight.IsHighlighted.value)
                {
                    tileEVs[i].highlight.IsHighlighted.value = false;
                }
            }
        }
    }
}
