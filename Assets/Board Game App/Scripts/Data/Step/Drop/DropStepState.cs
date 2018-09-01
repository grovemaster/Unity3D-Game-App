using Data.Enums.Piece.Side;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;

namespace Data.Step.Drop
{
    public struct DropStepState
    {
        public HandPieceEV HandPiece;
        public TileEV DestinationTile;
        public PieceSide Side;
    }
}
