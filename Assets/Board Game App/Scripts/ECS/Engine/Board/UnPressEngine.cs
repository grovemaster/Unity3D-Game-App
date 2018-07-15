using Data.Step.Board;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Board.Tile;
using Service.Piece;
using Svelto.ECS;

namespace ECS.Engine.Board
{
    class UnPressEngine : IStep<BoardPressStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref BoardPressStepState token, int condition)
        {
            UnPressPieces();
            UnPressTiles();
        }

        private void UnPressPieces()
        {
            PieceEV[] pieceEVs = PieceService.FindAllPieceEVs(entitiesDB);

            for (int i = 0; i < pieceEVs.Length; ++i)
            {
                if (pieceEVs[i].highlight.IsPressed.value)
                {
                    pieceEVs[i].highlight.IsPressed.value = false; // Will trigger a PiecePressEngine, but IsPressed check will stop it
                }
            }
        }

        private void UnPressTiles()
        {
            TileEV[] tileEVs = TileService.FindAllTileEVs(entitiesDB);

            for (int i = 0; i < tileEVs.Length; ++i)
            {
                if (tileEVs[i].highlight.IsPressed.value)
                {
                    tileEVs[i].highlight.IsPressed.value = false; // Will trigger a PiecePressEngine, but IsPressed check will stop it
                }
            }
        }
    }
}
