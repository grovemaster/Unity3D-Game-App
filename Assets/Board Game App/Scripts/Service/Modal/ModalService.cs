using ECS.EntityView.Modal;
using Service.Common;
using Svelto.ECS;

namespace Service.Modal
{
    public class ModalService
    {
        public ModalEV FindModalEV(IEntitiesDB entitiesDB)
        {
            return CommonService.FindAllEntities<ModalEV>(entitiesDB)[0];
        }
    }
}
