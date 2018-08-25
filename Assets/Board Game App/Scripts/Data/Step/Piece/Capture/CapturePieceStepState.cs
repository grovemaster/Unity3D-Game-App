using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;

namespace Data.Step.Piece.Capture
{
    public struct CapturePieceStepState
    {
        public PieceEV PieceToCapture;
        public PieceEV PieceToMove;
        public TileEV DestinationTile;
    }
}
