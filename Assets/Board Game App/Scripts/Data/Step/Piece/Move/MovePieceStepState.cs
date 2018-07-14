using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;

namespace Data.Step.Piece.Move
{
    public struct MovePieceStepState
    {
        public PieceEV pieceToMove;
        public TileEV destinationTile;
    }
}
