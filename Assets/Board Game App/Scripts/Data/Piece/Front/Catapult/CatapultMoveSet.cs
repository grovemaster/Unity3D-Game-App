using System.Collections.Generic;
using UnityEngine;

namespace Data.Piece.Front.Catapult
{
    class CatapultMoveSetTier1st : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>();
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

    class CatapultMoveSetTier2nd : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>();
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

    class CatapultMoveSetTier3rd : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>();
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
