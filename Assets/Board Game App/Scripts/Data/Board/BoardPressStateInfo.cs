using ECS.EntityView.Board.Tile;
using ECS.EntityView.Hand;
using ECS.EntityView.Piece;

namespace Scripts.Data.Board
{
    public struct BoardPressStateInfo
    {
        public PieceEV? piece;
        public TileEV? tile;
        public PieceEV? pieceAtDestination;
        public HandPieceEV? handPiece;
    }
}
