using Data.Step.Piece.Move;
using ECS.EntityView.Board.Tile;
using Service.Board.Tile;
using Svelto.ECS;

namespace ECS.Engine.Piece.Move
{
    class MovePieceCleanupEngine : IStep<MovePieceStepState>, IQueryingEntitiesEngine
    {
        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref MovePieceStepState token, int condition)
        {
            ClearPieceReferenceFromTiles();
        }

        private void ClearPieceReferenceFromTiles()
        {
            TileEV[] tileEVs = TileService.FindAllTileEVs(entitiesDB);

            for (int i = 0; i < tileEVs.Length; ++i)
            {
                tileEVs[i].tile.PieceRefEntityId = null;
            }
        }
    }
}
