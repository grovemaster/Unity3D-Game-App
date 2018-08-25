using Data.Enum;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Step
{
    public struct PressStepState
    {
        public int PieceEntityId;
        public PiecePressState PiecePressState;
        public List<Vector2> AffectedTiles;
    }
}
