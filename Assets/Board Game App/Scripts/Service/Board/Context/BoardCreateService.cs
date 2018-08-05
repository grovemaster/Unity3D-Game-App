using Data.Constants.Board;
using ECS.EntityDescriptor.Board.Tile;
using ECS.Implementor;
using ECS.Implementor.Board.Tile;
using PrefabUtil;
using Service.Common;
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
                    CreateTile(
                        CommonService.CalcTransformPosition(fileNum, rankNum),
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

            tile.transform.position = new Vector3(position.x, position.y, 0);
            tileImpl.Location = new Vector2(fileNum, rankNum);
        }
    }
}
