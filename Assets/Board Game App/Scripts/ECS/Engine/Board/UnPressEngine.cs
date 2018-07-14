using Data.Step.Board;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Board.Tile;
using Service.Piece;
using Svelto.ECS;

namespace ECS.Engine.Board
{
    class UnPressEngine : IStep<BoardPressState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref BoardPressState token, int condition)
        {
            UnPressPieces();
            UnPressTiles();
        }

        private void UnPressPieces()
        {
            int count;
            PieceEV[] pieceEVs = PieceService.FindAllPieceEVs(entitiesDB, out count);

            for (int i = 0; i < count; ++i)
            {
                if (pieceEVs[i].highlight.IsPressed.value)
                {
                    pieceEVs[i].highlight.IsPressed.value = false; // Will trigger a PiecePressEngine, but IsPressed check will stop it
                }
            }
        }

        private void UnPressTiles()
        {
            int count;
            TileEV[] tileEVs = TileService.FindAllTileEVs(entitiesDB, out count);

            for (int i = 0; i < count; ++i)
            {
                if (tileEVs[i].highlight.IsPressed.value)
                {
                    tileEVs[i].highlight.IsPressed.value = false; // Will trigger a PiecePressEngine, but IsPressed check will stop it
                }
            }
        }
    }
}
