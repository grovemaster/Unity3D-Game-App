using Data.Constants.Board;
using ECS.EntityDescriptor.Board.Tile;
using ECS.Implementor;
using ECS.Implementor.Board.Tile;
using PrefabUtil;
using Svelto.ECS;
using UnityEngine;

namespace Service.Board.Context
{
    public class BoardCreateService
    {
        private IEntityFactory entityFactory;
        private PrefabsDictionary prefabsDictionary;

        public BoardCreateService(IEntityFactory entityFactory)
        {
            this.entityFactory = entityFactory;
            prefabsDictionary = new PrefabsDictionary();
        }

        public void CreateBoard()
        {
            CreateBoardTiles();
        }

        private void CreateBoardTiles()
        {
            for (int rankNum = 0; rankNum < BoardConst.NUM_FILES_RANKS; rankNum++)
            {
                for (int fileNum = 0; fileNum < BoardConst.NUM_FILES_RANKS; ++fileNum)
                {
                    CreateTile(new Vector2(
                        BoardConst.TOP_LEFT_CORNER.x + BoardConst.TILE_SIZE * fileNum,
                        BoardConst.TOP_LEFT_CORNER.y + BoardConst.TILE_SIZE * rankNum),
                        fileNum,
                        rankNum);
                }
            }
        }

        private void CreateTile(Vector2 position, int fileNum, int rankNum)
        {
            var tile = prefabsDictionary.Instantiate("Board Tile");
            var tileImpl = tile.GetComponent<TileImpl>();
            entityFactory.BuildEntity<TileED>(tile.GetInstanceID(), tile.GetComponents<IImplementor>());

            tile.transform.position = position;
            tileImpl.Location = new Vector3(fileNum, rankNum, 0);
        }
    }
}
