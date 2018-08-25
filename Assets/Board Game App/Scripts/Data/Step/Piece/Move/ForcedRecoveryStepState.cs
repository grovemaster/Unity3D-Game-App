using ECS.EntityView.Piece;

namespace Data.Step.Piece.Move
{
    public struct ForcedRecoveryStepState
    {
        public PieceEV PieceMoved;
        public PieceEV? PieceCaptured;
    }
}
