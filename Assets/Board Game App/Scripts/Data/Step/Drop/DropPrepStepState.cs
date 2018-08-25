using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;

namespace Data.Step.Drop
{
    public struct DropPrepStepState
    {
        public HandPieceEV HandPiece;
        public TileEV DestinationTile;
    }
}
