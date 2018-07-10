using System.Collections.Generic;
using UnityEngine;

namespace Data.Piece.Front.Pawn
{
    class PawnMoveSetTier1st : IMoveSet
    {
        public List<Vector3> Single()
        {
            return new List<Vector3>(new Vector3[] { new Vector3(0, 1, 1) });
        }
    }
}
