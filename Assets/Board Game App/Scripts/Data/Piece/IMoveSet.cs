using System.Collections.Generic;
using UnityEngine;

namespace Data.Piece
{
    /**
     * Assume piece:
     * * Located at (0,0,1)
     * * Infinite board space on all sides
     * * No obstructions
     * * Piece is point UP
     */
    public interface IMoveSet
    {
        List<Vector3> Single();
    }
}
