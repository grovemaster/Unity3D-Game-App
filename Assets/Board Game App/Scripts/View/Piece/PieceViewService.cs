using System;
using Data.Enums.Piece;
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

        internal void ChangeIcon(GameObject pieceObject, PieceType pieceType)
        {
            string resourcesPath = CalcResourcesPath(pieceType);
            SpriteRenderer spriteRenderer = pieceObject.transform.Find("Icon").GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>(resourcesPath);
        }

        private string CalcResourcesPath(PieceType pieceType)
        {

            switch (pieceType)
            {
                // Front-Back Combos
                case PieceType.COMMANDER:
                    return PIECE_ICON_PATH_BASE + "Commander/Commander";

                case PieceType.CAPTAIN:
                    return PIECE_ICON_PATH_BASE + "Captain/Captain";
                case PieceType.PISTOL:
                    return PIECE_ICON_PATH_BASE + "Pistol/Pistol";

                case PieceType.SAMURAI:
                    return PIECE_ICON_PATH_BASE + "Samurai/Samurai";
                case PieceType.PIKE:
                    return PIECE_ICON_PATH_BASE + "Pike/Pike";

                case PieceType.SPY:
                    return PIECE_ICON_PATH_BASE + "Spy/Spy";
                case PieceType.CLANDESTINITE:
                    return PIECE_ICON_PATH_BASE + "Clandestinite/Clandestinite";

                case PieceType.CATAPULT:
                    return PIECE_ICON_PATH_BASE + "Catapult/Catapult";
                case PieceType.FORTRESS:
                    return PIECE_ICON_PATH_BASE + "Fortress/Fortress";
                case PieceType.LANCE:
                    return PIECE_ICON_PATH_BASE + "Lance/Lance";

                case PieceType.HIDDEN_DRAGON:
                    return PIECE_ICON_PATH_BASE + "Hidden Dragon/Hidden Dragon";
                case PieceType.DRAGON_KING:
                    return PIECE_ICON_PATH_BASE + "Dragon King/Dragon King";

                case PieceType.PRODIGY:
                    return PIECE_ICON_PATH_BASE + "Prodigy/Prodigy";
                case PieceType.PHOENIX:
                    return PIECE_ICON_PATH_BASE + "Phoenix/Phoenix";

                case PieceType.BOW:
                    return PIECE_ICON_PATH_BASE + "Bow/Bow";
                case PieceType.ARROW:
                    return PIECE_ICON_PATH_BASE + "Arrow/Arrow";

                case PieceType.PAWN:
                    return PIECE_ICON_PATH_BASE + "Pawn/Pawn";
                case PieceType.BRONZE:
                    return PIECE_ICON_PATH_BASE + "Bronze/Bronze";
                case PieceType.SILVER:
                    return PIECE_ICON_PATH_BASE + "Silver/Silver";
                case PieceType.GOLD:
                    return PIECE_ICON_PATH_BASE + "Gold/Gold";
                default:
                    throw new InvalidOperationException("Invalid PieceType when creating IPieceData");
            }

            return PIECE_ICON_PATH_BASE + "Pawn/icons8-pawn"; ;
        }
    }
}
