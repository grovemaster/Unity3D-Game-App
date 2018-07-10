using Data.Enum;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Step
{
    public struct PressState
    {
        public int pieceEntityId;
        public PiecePressState piecePressState;
        public List<Vector3> affectedTiles;
    }
}
