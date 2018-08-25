﻿using System.Collections.Generic;
using UnityEngine;

namespace Data.Piece.Back.Bronze
{
    class BronzeMoveSetTier1st : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>(new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
        });

        private static readonly List<Vector2> jump = new List<Vector2>();

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

    class BronzeMoveSetTier2nd : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>(new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
        });

        private static readonly List<Vector2> jump = new List<Vector2>();

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

    class BronzeMoveSetTier3rd : IMoveSet
    {
        private static readonly List<Vector2> single = new List<Vector2>(new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(-1, 0),
        });

        private static readonly List<Vector2> jump = new List<Vector2>();

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
