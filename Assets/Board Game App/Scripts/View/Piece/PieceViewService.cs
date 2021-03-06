﻿using System;
using Data.Enums.Piece;
using Data.Enums.Player;
using UnityEngine;

namespace View.Piece
{
    public class PieceViewService
    {
        private static readonly string PIECE_ICON_PATH_BASE = "Images/Piece/";

        public void ChangePieceUpSideText(GameObject pieceGameObject, PieceType pieceType)
        {
            char[] charArray = pieceType.ToString().Substring(0, 2).ToCharArray();
            string twoChars = charArray[0].ToString() + charArray[1].ToString().ToLower();

            TextMesh upSideText = pieceGameObject.transform.Find("Up Side Text").GetComponent<TextMesh>();
            upSideText.text = twoChars;

            upSideText.text = "";
        }

        internal void ChangePieceDownSideText(GameObject pieceGameObject, PieceType pieceType)
        {
            char[] charArray = pieceType.ToString().Substring(0, 2).ToCharArray();
            string twoChars = charArray[0].ToString() + charArray[1].ToString().ToLower();

            if (pieceType == PieceType.COMMANDER)
            {
                twoChars = ""; // Exception to general rule
            }

            TextMesh upSideText = pieceGameObject.transform.Find("Back Side Text").GetComponent<TextMesh>();
            upSideText.text = twoChars;

            upSideText.text = "";
        }

        internal void ChangeIcon(
            GameObject pieceGameObject, PlayerColor teamColor, PieceType pieceType, PieceType back)
        {
            string resourcesPath = CalcResourcesPath(pieceType, back);
            SpriteRenderer spriteRenderer = pieceGameObject.transform.Find("Icon").GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>(resourcesPath);

            if (teamColor == PlayerColor.WHITE)
            {
                spriteRenderer.transform.localRotation = Quaternion.Euler(180, 180, 0);
            }
        }

        internal void ChangePlayerBorder(GameObject piece, PlayerColor playerColor)
        {
            var sprite = piece.transform.Find("Sprite").GetComponentInChildren<SpriteRenderer>();
            sprite.sprite = playerColor == PlayerColor.BLACK
                ? Resources.Load<Sprite>("Images/Piece/Black Piece Border")
                : Resources.Load<Sprite>("Images/Piece/White Piece Border");
        }

        internal string CalcResourcesPath(PieceType pieceType, PieceType back)
        {
            switch (pieceType)
            {
                // Front-Back Combos
                case PieceType.COMMANDER:
                    return PIECE_ICON_PATH_BASE + "Commander/Commander_en";

                case PieceType.CAPTAIN:
                    return PIECE_ICON_PATH_BASE + "Captain/Captain_en";
                case PieceType.PISTOL:
                    return PIECE_ICON_PATH_BASE + "Pistol/Pistol_en";

                case PieceType.SAMURAI:
                    return PIECE_ICON_PATH_BASE + "Samurai/Samurai_en";
                case PieceType.PIKE:
                    return PIECE_ICON_PATH_BASE + "Pike/Pike_en";

                case PieceType.SPY:
                    return PIECE_ICON_PATH_BASE + "Spy/Spy_en";
                case PieceType.CLANDESTINITE:
                    return PIECE_ICON_PATH_BASE + "Clandestinite/Clandestinite_en";

                case PieceType.CATAPULT:
                    return PIECE_ICON_PATH_BASE + "Catapult/Catapult_en";
                case PieceType.FORTRESS:
                    return PIECE_ICON_PATH_BASE + "Fortress/Fortress_en";
                case PieceType.LANCE:
                    return PIECE_ICON_PATH_BASE + "Lance/Lance_en";

                case PieceType.HIDDEN_DRAGON:
                    return PIECE_ICON_PATH_BASE + "Hidden Dragon/Hidden Dragon_en";
                case PieceType.DRAGON_KING:
                    return PIECE_ICON_PATH_BASE + "Dragon King/Dragon King_en";

                case PieceType.PRODIGY:
                    return PIECE_ICON_PATH_BASE + "Prodigy/Prodigy_en";
                case PieceType.PHOENIX:
                    return PIECE_ICON_PATH_BASE + "Phoenix/Phoenix_en";

                case PieceType.BOW:
                    return PIECE_ICON_PATH_BASE + "Bow/Bow_en";
                case PieceType.ARROW:
                    return PIECE_ICON_PATH_BASE + "Arrow/Arrow_en";

                case PieceType.PAWN:
                    switch (back)
                    {
                        case PieceType.BRONZE:
                            return PIECE_ICON_PATH_BASE + "Pawn/Pawn Bronze_en";
                        case PieceType.SILVER:
                            return PIECE_ICON_PATH_BASE + "Pawn/Pawn Silver_en";
                        case PieceType.GOLD:
                            return PIECE_ICON_PATH_BASE + "Pawn/Pawn Gold_en";
                        default:
                            throw new InvalidOperationException("Invalid back PieceType for Pawn when locating Piece sprite");
                    }
                case PieceType.BRONZE:
                    return PIECE_ICON_PATH_BASE + "Bronze/Bronze_en";
                case PieceType.SILVER:
                    return PIECE_ICON_PATH_BASE + "Silver/Silver_en";
                case PieceType.GOLD:
                    return PIECE_ICON_PATH_BASE + "Gold/Gold_en";
                default:
                    throw new InvalidOperationException("Invalid PieceType when locating Piece sprite");
            }
        }
    }
}
