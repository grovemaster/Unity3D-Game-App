using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.jonaus.bgapp.board
{
    public class BoardMaker : MonoBehaviour
    {
        private static Vector2 TOP_LEFT_CORNER = new Vector2(0, 0);
        private static float TILE_SIZE = 8.3f; // Should determine size of board tiles more effectively once I learn more about Unity3D
        private static int NUM_ROWS_COLS = 9;

        void Start()
        {
            createBoardTiles();
        }

        private void createBoardTiles()
        {
            for (int rowNum = 0; rowNum < NUM_ROWS_COLS; rowNum++)
            {
                for (int colNum = 0; colNum < NUM_ROWS_COLS; ++colNum)
                {
                    createBoardTile(new Vector2(TOP_LEFT_CORNER.x + TILE_SIZE * colNum, TOP_LEFT_CORNER.y + TILE_SIZE * rowNum));
                }
            }
        }

        private void createBoardTile(Vector2 position)
        {
            GameObject boardTile = Instantiate(Resources.Load<GameObject>("Prefabs/Board/Tile/Board Tile"));
            boardTile.transform.position = position;
        }
    }
}