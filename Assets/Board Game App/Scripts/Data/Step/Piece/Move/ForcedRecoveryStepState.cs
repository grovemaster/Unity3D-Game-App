using ECS.EntityView.Piece;

namespace Data.Step.Piece.Move
{
    public struct ForcedRecoveryStepState
    {
        public PieceEV pieceMoved;
        public PieceEV? pieceCaptured;
    }
}
