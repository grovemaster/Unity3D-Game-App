using System.Collections.Generic;
using UnityEngine;

namespace Data.Piece.Front.Pawn
{
    class PawnMoveSetTier1st : IMoveSet
    {
        private static readonly List<Vector3> single = new List<Vector3>(new Vector3[] { new Vector3(0, 1, 1) });

        public List<Vector3> Single()
        {
            return single;
        }
    }

    class PawnMoveSetTier2nd : IMoveSet
    {
        private static readonly List<Vector3> single = new List<Vector3>(new Vector3[]
        {
            new Vector3(0, 1, 1),
            new Vector3(-2, 0, 1),
            new Vector3(2, 0, 1)
        });

        public List<Vector3> Single()
        {
            return single;
        }
    }

    class PawnMoveSetTier3rd : IMoveSet
    {
        private static readonly List<Vector3> single = new List<Vector3>(new Vector3[]
        {
            new Vector3(-2, 0, 1),
            new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(2, 0, 1)
        });

        public List<Vector3> Single()
        {
            return single;
        }
    }
}
