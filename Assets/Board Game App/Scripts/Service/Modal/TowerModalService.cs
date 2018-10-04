using ECS.EntityView.Modal;
using Service.Common;
using Svelto.ECS;

namespace Service.Modal
{
    public class TowerModalService
    {
        public TowerModalEV FindModalEV(IEntitiesDB entitiesDB)
        {
            return CommonService.FindAllEntities<TowerModalEV>(entitiesDB)[0];
        }

        public void DisplayModal(TowerModalEV modal)
        {
            modal.Visibility.IsVisible.value = true;
        }
    }
}
