using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;

namespace Data.Step.Drop
{
    public struct DropPrepStepState
    {
        public PieceEV PieceToDrop;
        public TileEV DestinationTile;
    }
}
