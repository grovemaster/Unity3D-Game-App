namespace Data.Check.PreviousMove
{
    public struct PreviousMoveState
    {
        public PreviousPieceState pieceToMove; // location, topOfTower
        public PreviousPieceState? pieceBelow; // topOfTower // If new topOfTower is enemy piece
        //public PreviousPieceState? pieceAbove; // tier // Immobile capture causes tier adjustment to topOfTower enemy piece
        public PreviousPieceState? pieceCaptured; // location, topOfTower
    }
}
