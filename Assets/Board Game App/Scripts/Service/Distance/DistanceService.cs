using System;
using UnityEngine;

namespace Service.Distance
{
    public class DistanceService
    {
        // Absolute Distance defined as sum of absolute(total x distance) + absolute(total y distance)
        // between the two pieces
        public float CalcAbsoluteDistance(Vector2 location1, Vector2 location2)
        {
            return Math.Abs(location1.x - location2.x)
                + Math.Abs(location1.y - location2.y);
        }
    }
}
