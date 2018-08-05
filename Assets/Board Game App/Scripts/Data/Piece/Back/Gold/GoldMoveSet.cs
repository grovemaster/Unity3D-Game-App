using System.Collections.Generic;
using UnityEngine;

namespace Data.Piece.Back.Gold
{
    class GoldMoveSetTier1st : IMoveSet
    {
        private static readonly List<Vector3> single = new List<Vector3>(new Vector3[]
        {
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, -1, 1),
            new Vector3(-1, 0, 1),
            new Vector3(-1, 1, 1)
        });

        public List<Vector3> Single()
        {
            return single;
        }
    }

    class GoldMoveSetTier2nd : IMoveSet
    {
        private static readonly List<Vector3> single = new List<Vector3>(new Vector3[]
        {
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, -1, 1),
            new Vector3(-1, 0, 1),
            new Vector3(-1, 1, 1)
        });

        public List<Vector3> Single()
        {
            return single;
        }
    }

    class GoldMoveSetTier3rd : IMoveSet
    {
        private static readonly List<Vector3> single = new List<Vector3>(new Vector3[]
        {
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, -1, 1),
            new Vector3(-1, 0, 1),
            new Vector3(-1, 1, 1)
        });

        public List<Vector3> Single()
        {
            return single;
        }
    }
}
