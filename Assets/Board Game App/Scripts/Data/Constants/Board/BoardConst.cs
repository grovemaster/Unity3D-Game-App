using UnityEngine;

namespace Data.Constants.Board
{
    public static class BoardConst
    {
        public static readonly Vector2 TOP_LEFT_CORNER = new Vector2(0, 0);
        public static readonly Vector2 HAND_LOCATION = new Vector2(-1, -1);
        public static readonly Vector3 HAND_PIECE_BLACK_OFFSET = new Vector3(0, -13, -5);
        public static readonly Vector3 HAND_PIECE_WHITE_OFFSET = new Vector3(0, 75, -5);
        public static readonly Vector3 TOWER_OFFSET = new Vector3(1f, 1f, -1f);
        public const float TILE_SIZE = 8.3f; // Should determine size of board tiles more effectively once I learn more about Unity3D
        public const int NUM_FILES_RANKS = 9;
        public const float HAND_PIECE_X_SPACE = 10f;
        public const float HAND_PIECE_Y_SPACE = 10f;
        public const int NUM_HAND_PIECES_IN_ROW = 7;
    }
}
