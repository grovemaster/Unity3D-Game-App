using System.Collections.Generic;
using UnityEngine;

namespace Data.Piece.Front.Bow
{
    class BowMoveSetTier1st : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>();

        private static readonly List<Vector2> jump = new List<Vector2>(new Vector2[]
        {
            new Vector2(2, 0),
            new Vector2(-2, 0),
            new Vector2(0, 2)
        });

        public List<Vector2> Single
        {
            get
            {
                return single.ConvertAll(vec => new Vector2(vec.x, vec.y));
            }
        }

        public List<Vector2> Jump
        {
            get
            {
                return jump.ConvertAll(vec => new Vector2(vec.x, vec.y)); ;
            }
        }
    }

    class BowMoveSetTier2nd : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>(new Vector2[]
        {
            new Vector2(0, -1),
            new Vector2(0, 1)
        });

        private static readonly List<Vector2> jump = new List<Vector2>(new Vector2[]
        {
            new Vector2(2, 2),
            new Vector2(-2, 2)
        });

        public List<Vector2> Single
        {
            get
            {
                return single.ConvertAll(vec => new Vector2(vec.x, vec.y));
            }
        }

        public List<Vector2> Jump
        {
            get
            {
                return jump.ConvertAll(vec => new Vector2(vec.x, vec.y)); ;
            }
        }
    }

    class BowMoveSetTier3rd : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>();

        private static readonly List<Vector2> jump = new List<Vector2>(new Vector2[]
        {
            new Vector2(2, 2),
            new Vector2(2, 0),
            new Vector2(0, -2),
            new Vector2(-2, 0),
            new Vector2(-2, 2)
        });

        public List<Vector2> Single
        {
            get
            {
                return single.ConvertAll(vec => new Vector2(vec.x, vec.y));
            }
        }

        public List<Vector2> Jump
        {
            get
            {
                return jump.ConvertAll(vec => new Vector2(vec.x, vec.y)); ;
            }
        }
    }
}
