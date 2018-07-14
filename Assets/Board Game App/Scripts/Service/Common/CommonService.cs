
using Svelto.ECS;

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

        public static T[] FindAllEntities<T>(IEntitiesDB entitiesDB, out int count) where T : IEntityViewStruct
        {
            return entitiesDB.QueryEntities<T>(out count);
        }

        public static T FindEntityById<T>(int? entityId, IEntitiesDB entitiesDB) where T : IEntityViewStruct
        {
            T returnValue = default(T);

            if (entityId != null && entityId != 0)
            {
                returnValue = FindEntity<T>((int)entityId, entitiesDB);
            }

            return returnValue;
        }
    }
}
