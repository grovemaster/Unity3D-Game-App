using Data.Enum;
using ECS.EntityView.Board.Tile;
using ECS.EntityView.Piece;

namespace Data.Step.Board
{
    public struct BoardPressState
    {
        public int? pieceEntityId;
        public int? tileEntityId;
    }
}
