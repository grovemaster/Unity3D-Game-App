using Data.Enum;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Step
{
    public struct PressStepState
    {
        public int pieceEntityId;
        public PiecePressState piecePressState;
        public List<Vector2> affectedTiles;
    }
}
