using Data.Step.Turn;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;
using Service.Board.Tile;
using Service.Hand;
using Service.Piece.Find;
using Svelto.ECS;

namespace ECS.Engine.Board
{
    class UnHighlightEngine : IStep<TurnEndStepState>, IQueryingEntitiesEngine
    {
        private PieceFindService pieceFindService = new PieceFindService();
        private HandService handService = new HandService();
        private TileService tileService = new TileService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref TurnEndStepState token, int condition)
        {
            UnHighlightObjects();
        }

        private void UnHighlightObjects()
        {
            UnHighlightPieces();
            UnHighlightTiles();
            UnHighlightHandPieces();
        }

        private void UnHighlightPieces()
        {
            PieceEV[] pieceEVs = pieceFindService.FindAllBoardPieces(entitiesDB);

            for (int i = 0; i < pieceEVs.Length; ++i)
            {
                if (pieceEVs[i].Highlight.IsHighlighted)
                {
                    entitiesDB.ExecuteOnEntity(
                        pieceEVs[i].ID,
                        (ref PieceEV pieceToChange) =>
                        {
                            pieceToChange.Highlight.IsHighlighted = false;
                            pieceToChange.Highlight.CurrentColorStates.Clear();
                        });

                    pieceEVs[i].ChangeColorTrigger.PlayChangeColor = true;
                }
            }
        }

        private void UnHighlightTiles()
        {
            TileEV[] tileEVs = tileService.FindAllTileEVs(entitiesDB);

            for (int i = 0; i < tileEVs.Length; ++i)
            {
                // TODO Sloppy coding; if gated with an if statement, captured piece's range highlight remains.
                entitiesDB.ExecuteOnEntity(
                    tileEVs[i].ID,
                    (ref TileEV tileToChange) =>
                    {
                        tileToChange.Highlight.IsHighlighted = false;
                        tileToChange.Highlight.CurrentColorStates.Clear();
                    });

                tileEVs[i].ChangeColorTrigger.PlayChangeColor = true;
            }
        }

        private void UnHighlightHandPieces()
        {
            handService.UnHighlightAllHandPieces(entitiesDB);
        }
    }
}
