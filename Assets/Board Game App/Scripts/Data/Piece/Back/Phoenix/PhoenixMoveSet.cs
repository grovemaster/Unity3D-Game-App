using System.Collections.Generic;
using UnityEngine;

namespace Data.Piece.Back.Phoenix
{
    class PhoenixMoveSetTier1st : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>(new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0)
        });

        private static readonly List<Vector2> jump = new List<Vector2>();

        private static readonly List<Vector2> line = new List<Vector2>(new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, -1),
            new Vector2(-1, -1),
            new Vector2(-1, 1)
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

        public List<Vector2> Line
        {
            get
            {
                return line.ConvertAll(vec => new Vector2(vec.x, vec.y)); ;
            }
        }
    }

    class PhoenixMoveSetTier2nd : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>(new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0)
        });

        private static readonly List<Vector2> jump = new List<Vector2>();
        private static readonly List<Vector2> line = new List<Vector2>();

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

        public List<Vector2> Line
        {
            get
            {
                return line.ConvertAll(vec => new Vector2(vec.x, vec.y)); ;
            }
        }
    }

    class PhoenixMoveSetTier3rd : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>(new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0)
        });

        private static readonly List<Vector2> jump = new List<Vector2>();
        private static readonly List<Vector2> line = new List<Vector2>();

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

        public List<Vector2> Line
        {
            get
            {
                return line.ConvertAll(vec => new Vector2(vec.x, vec.y)); ;
            }
        }
    }
}
