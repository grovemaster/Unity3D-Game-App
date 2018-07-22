using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;

namespace Data.Step.Piece.Capture
{
    public struct CapturePieceStepState
    {
        public PieceEV pieceToCapture;
        public PieceEV pieceToMove;
        public TileEV destinationTile;
    }
}
