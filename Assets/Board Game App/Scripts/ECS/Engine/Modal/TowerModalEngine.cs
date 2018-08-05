using Data.Enum.Modal;
using Data.Step.Piece.Click;
using ECS.EntityView.Modal;
using ECS.EntityView.Piece;
using Service.Modal;
using Service.Piece;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Engine.Modal
{
    class TowerModalEngine : IStep<ClickPieceStepState>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref ClickPieceStepState token, int condition)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            List<PieceEV> piecesAtLocation = PieceService.FindPiecesByLocation(
                token.clickedPiece.location.Location, entitiesDB);

            SetModalOptions(modal, piecesAtLocation);
            DisplayModal(modal);
        }

        private void DisplayModal(ModalEV modal)
        {
            modal.visibility.IsVisible.value = true;
        }

        private void SetModalOptions(ModalEV modal, List<PieceEV> piecesAtLocation)
        {
            ModalType modalType = CalcModalType(piecesAtLocation);

            SetModalType(modal, modalType);
            SetTierOptions(modal, modalType, piecesAtLocation);
        }

        private ModalType CalcModalType(List<PieceEV> piecesAtLocation)
        {
            return piecesAtLocation.Count < 3 ? ModalType.TOWER_2ND_TIER : ModalType.TOWER_3RD_TIER;
        }

        private string CalcButtonText(PieceEV piece)
        {
            // For now, every back piece is Bronze (B)
            string returnValue = "(" + piece.playerOwner.PlayerColor.ToString() + ") " + piece.piece.PieceType.ToString() + "(B)";

            return returnValue;
        }

        private void SetModalType(ModalEV modal, ModalType modalType)
        {
            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modalToChange.type.Type = modalType;
                });
        }

        private void SetTierOptions(ModalEV modal, ModalType modalType, List<PieceEV> piecesAtLocation)
        {
            PieceEV pieceTier1 = piecesAtLocation[0];
            PieceEV pieceTier2 = piecesAtLocation[1];
            PieceEV? pieceTier3 = modalType == ModalType.TOWER_3RD_TIER ? (PieceEV?)piecesAtLocation[2] : null;

            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modalToChange.tier1.Enabled = pieceTier1.tier.TopOfTower;
                    modalToChange.tier1.Name = CalcButtonText(pieceTier1);
                    modalToChange.tier1.ReferencedPieceId = pieceTier1.ID.entityID;

                    modalToChange.tier2.Enabled = pieceTier2.tier.TopOfTower;
                    modalToChange.tier2.Name = CalcButtonText(pieceTier2);
                    modalToChange.tier2.ReferencedPieceId = pieceTier2.ID.entityID;

                    if (pieceTier3.HasValue)
                    {
                        modalToChange.tier3.Enabled = pieceTier3.Value.tier.TopOfTower;
                        modalToChange.tier3.Name = CalcButtonText(pieceTier3.Value);
                        modalToChange.tier3.ReferencedPieceId = pieceTier3.Value.ID.entityID;
                    }
                });
        }
    }
}
