using ECS.EntityView.Piece;

namespace Data.Step.Piece.Ability
{
    public struct DeterminePostMoveStepState
    {
        public PieceEV PieceMoved;
        public PieceEV? PieceCaptured;
    }
}
