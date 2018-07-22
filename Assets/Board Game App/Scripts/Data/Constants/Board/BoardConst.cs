using UnityEngine;

namespace Data.Constants.Board
{
    public static class BoardConst
    {
        public static readonly Vector2 TOP_LEFT_CORNER = new Vector2(0, 0);
        public static readonly Vector3 HAND_LOCATION = new Vector3(-1, -1, -1);
        public static readonly Vector3 HAND_PIECE_BLACK_OFFSET = new Vector3(0, -13, -5);
        public static readonly Vector3 HAND_PIECE_WHITE_OFFSET = new Vector3(0, 75, -5);
        public const float TILE_SIZE = 8.3f; // Should determine size of board tiles more effectively once I learn more about Unity3D
        public const int NUM_FILES_RANKS = 9;
    }
}
