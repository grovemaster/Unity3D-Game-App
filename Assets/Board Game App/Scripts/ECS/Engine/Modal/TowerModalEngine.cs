﻿using Data.Enum.Modal;
using Data.Enum.Player;
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
                token.clickedPiece.Location.Location, entitiesDB);

            SetModalOptions(modal, piecesAtLocation);
            modalService.DisplayModal(modal);
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
            string returnValue = "(" + piece.PlayerOwner.PlayerColor.ToString() + ") " + piece.Piece.PieceType.ToString() + "(B)";

            return returnValue;
        }

        private void SetModalType(ModalEV modal, ModalType modalType)
        {
            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modalToChange.Type.Type = modalType;
                });
        }

        private void SetTierOptions(ModalEV modal, ModalType modalType, List<PieceEV> piecesAtLocation)
        {
            PieceEV pieceTier1 = piecesAtLocation[0];
            PieceEV pieceTier2 = piecesAtLocation[1];
            PieceEV? pieceTier3 = modalType == ModalType.TOWER_3RD_TIER ? (PieceEV?)piecesAtLocation[2] : null;
            PlayerColor topPlayerColor = pieceTier3.HasValue ?
                pieceTier3.Value.PlayerOwner.PlayerColor : pieceTier2.PlayerOwner.PlayerColor;
            bool immobileCapturePossible = pieceTier2.PlayerOwner.PlayerColor != pieceTier1.PlayerOwner.PlayerColor
                || (pieceTier3.HasValue && pieceTier2.PlayerOwner.PlayerColor != pieceTier3.Value.PlayerOwner.PlayerColor);

            entitiesDB.ExecuteOnEntity(
                modal.ID,
                (ref ModalEV modalToChange) =>
                {
                    modalToChange.ImmobileCaptureState.ImmobileCaptureDesignated = false;

                    modalToChange.Tier1.Enabled = pieceTier1.Tier.TopOfTower
                        || (immobileCapturePossible
                            && pieceTier1.PlayerOwner.PlayerColor != pieceTier2.PlayerOwner.PlayerColor
                            && pieceTier1.PlayerOwner.PlayerColor != topPlayerColor);
                    modalToChange.Tier1.Name = CalcButtonText(pieceTier1);
                    modalToChange.Tier1.ReferencedPieceId = pieceTier1.ID.entityID;

                    modalToChange.Tier2.Enabled = pieceTier2.Tier.TopOfTower
                        || (immobileCapturePossible
                            && (pieceTier2.PlayerOwner.PlayerColor != pieceTier1.PlayerOwner.PlayerColor
                            || (!pieceTier3.HasValue || pieceTier2.PlayerOwner.PlayerColor != pieceTier3.Value.PlayerOwner.PlayerColor))
                            && pieceTier2.PlayerOwner.PlayerColor != topPlayerColor);
                    modalToChange.Tier2.Name = CalcButtonText(pieceTier2);
                    modalToChange.Tier2.ReferencedPieceId = pieceTier2.ID.entityID;

                    if (pieceTier3.HasValue)
                    {
                        modalToChange.Tier3.Enabled = pieceTier3.Value.Tier.TopOfTower;
                        modalToChange.Tier3.Name = CalcButtonText(pieceTier3.Value);
                        modalToChange.Tier3.ReferencedPieceId = pieceTier3.Value.ID.entityID;
                    }
                    else
                    {
                        modalToChange.Tier3.Enabled = false;
                    }
                });
        }
    }
}
