
using Data.Constants.Board;
using Svelto.ECS;
using System.Linq;
using UnityEngine;

namespace Service.Common
{
    public static class CommonService
    {
        public static T FindEntity<T>(int entityId, IEntitiesDB entitiesDB) where T : IEntityViewStruct
        {
            uint index;
            var entityViews = entitiesDB.QueryEntitiesAndIndex<T>(new EGID(entityId), out index);

            return entityViews[index];
        }

        public static T[] FindAllEntities<T>(IEntitiesDB entitiesDB) where T : IEntityViewStruct
        {
            int count;
            return entitiesDB.QueryEntities<T>(out count).Where(ev => ev.ID.entityID != 0).ToArray();
        }

        public static T FindEntityById<T>(int? entityId, IEntitiesDB entitiesDB) where T : IEntityViewStruct
        {
            T returnValue = default(T);

            if (entityId.HasValue && entityId != 0)
            {
                returnValue = FindEntity<T>((int)entityId, entitiesDB);
            }

            return returnValue;
        }

        public static Vector3 CalcTransformPosition(int fileNum, int rankNum, int z)
        {
            return new Vector3(
                BoardConst.TOP_LEFT_CORNER.x + BoardConst.TILE_SIZE * fileNum,
                BoardConst.TOP_LEFT_CORNER.y + BoardConst.TILE_SIZE * rankNum,
                z);
        }
    }
}
