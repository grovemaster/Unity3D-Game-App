using Data.Step.Turn;
using ECS.EntityView.Board.Tile;
using Service.Board.Tile;
using Svelto.ECS;

namespace ECS.Engine.Piece.Move
{
    class MovePieceCleanupEngine : IStep<TurnEndStepState>, IQueryingEntitiesEngine
    {
        private TileService tileService = new TileService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref TurnEndStepState token, int condition)
        {
            ClearPieceReferenceFromTiles();
        }

        private void ClearPieceReferenceFromTiles()
        {
            TileEV[] tileEVs = tileService.FindAllTileEVs(entitiesDB);

            for (int i = 0; i < tileEVs.Length; ++i)
            {
                tileEVs[i].Tile.PieceRefEntityId = null;
            }
        }
    }
}
