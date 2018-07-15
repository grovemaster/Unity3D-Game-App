using ECS.EntityView.Board.Tile;
using Service.Common;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Service.Board.Tile
{
    public static class TileService
    {
        public static TileEV FindTileEV(int entityId, IEntitiesDB entitiesDB)
        {
            return CommonService.FindEntity<TileEV>(entityId, entitiesDB);
        }

        public static TileEV FindTileEV(Vector3 location, IEntitiesDB entitiesDB)
        {
            var tiles = FindAllTileEVs(entitiesDB);

            for (int i = 0; i < tiles.Length; ++i)
            {
                if (tiles[i].location.Location == location)
                {
                    return tiles[i];
                }
            }

            throw new ArgumentOutOfRangeException("No matching location for finding TilEV");
        }

        public static TileEV[] FindAllTileEVs(IEntitiesDB entitiesDB)
        {
            return CommonService.FindAllEntities<TileEV>(entitiesDB);
        }

        public static TileEV? FindTileEVById(int? entityId, IEntitiesDB entitiesDB)
        {
            TileEV returnValue = CommonService.FindEntityById<TileEV>(entityId, entitiesDB);

            return returnValue.ID.entityID != 0 ? (TileEV?)returnValue : null;
        }
    }
}
