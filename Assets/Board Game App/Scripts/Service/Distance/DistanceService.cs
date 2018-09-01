using Data.Enums;
using Data.Enums.Player;
using Service.Directions;
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

        // If locationToCompare is "ahead" or equal rank of locationBase, return true, else false
        // "Ahead" being defined as closer to relative end of board while being in same file
        public bool IsAhead(Vector2 locationBase, Vector2 locationToCompare, PlayerColor playerColor)
        {
            return locationBase == locationToCompare ||
                (locationBase.x == locationToCompare.x
                && (DirectionService.CalcDirection(playerColor) == Direction.UP
                    ? locationBase.y <= locationToCompare.y : locationBase.y >= locationToCompare.y));
        }
    }
}
