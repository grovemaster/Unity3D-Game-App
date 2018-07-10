using UnityEngine;

namespace Data.Constants.Board
{
    public class BoardConst
    {
        public static Vector2 TOP_LEFT_CORNER = new Vector2(0, 0);
        public const float TILE_SIZE = 8.3f; // Should determine size of board tiles more effectively once I learn more about Unity3D
        public const int NUM_FILES_RANKS = 9;
    }
}
