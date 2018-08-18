using Data.Enum.Player;
using Data.Step.Modal;
using ECS.EntityView.Modal;
using ECS.EntityView.Piece;
using Service.Modal;
using Service.Piece.Find;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Engine.Modal.ImmobileCapture
{
    class DesignateImmobileCaptureEngine : IStep<ImmobileCaptureStepState>, IQueryingEntitiesEngine
    {
        private ModalService modalService = new ModalService();
        private PieceFindService pieceFindService = new PieceFindService();

        public IEntitiesDB entitiesDB { private get; set; }

        public void Ready()
        { }

        public void Step(ref ImmobileCaptureStepState token, int condition)
        {
            ModalEV modal = modalService.FindModalEV(entitiesDB);
            SetModalOptions(modal);
            UpdateModalView(modal);
        }

        private void SetModalOptions(ModalEV modal)
        {
            PieceEV pieceTier1 = pieceFindService.FindPieceEVById(modal.Tier1.ReferencedPieceId, entitiesDB).Value;
            List<PieceEV> piecesAtLocation = pieceFindService.FindPiecesByLocation(pieceTier1.Location.Location, entitiesDB);

            PieceEV topPiece = pieceFindService.FindTopPieceByLocation(pieceTier1.Location.Location, entitiesDB).Value;
            PlayerColor colorToEnable = topPiece.PlayerOwner.PlayerColor;

            PieceEV pieceTier2 = piecesAtLocation[1];
            PieceEV? pieceTier3 = piecesAtLocation.Count > 2 ? (PieceEV?)piecesAtLocation[2] : null;

            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modalToChange.ImmobileCaptureState.ImmobileCaptureDesignated = true;

                    modalToChange.Tier1.Enabled = pieceTier1.PlayerOwner.PlayerColor == colorToEnable
                        && pieceTier1.PlayerOwner.PlayerColor != pieceTier2.PlayerOwner.PlayerColor;

                    modalToChange.Tier2.Enabled = pieceTier2.PlayerOwner.PlayerColor == colorToEnable
                        && (pieceTier2.PlayerOwner.PlayerColor != pieceTier1.PlayerOwner.PlayerColor
                        || (pieceTier3.HasValue && pieceTier2.PlayerOwner.PlayerColor != pieceTier3.Value.PlayerOwner.PlayerColor));

                    if (pieceTier3.HasValue)
                    {
                        modalToChange.Tier3.Enabled = pieceTier3.Value.PlayerOwner.PlayerColor == colorToEnable
                        && pieceTier3.Value.PlayerOwner.PlayerColor != pieceTier2.PlayerOwner.PlayerColor;
                    }
                    else
                    {
                        modalToChange.Tier3.Enabled = false;
                    }
                });
        }

        private void UpdateModalView(ModalEV modal)
        {
            modal.Visibility.IsVisible.value = true;
        }
    }
}
