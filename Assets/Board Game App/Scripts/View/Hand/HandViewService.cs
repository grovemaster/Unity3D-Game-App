using Data.Enums.Piece;
using Data.Enums.Player;
using UnityEngine;
using View.Piece;

namespace View.Hand
{
    public class HandViewService
    {
        private PieceViewService pieceViewService = new PieceViewService();

        public void ChangePieceUpSideText(GameObject handPieceGameObject, PieceType pieceType)
        {
            char[] charArray = pieceType.ToString().Substring(0, 2).ToCharArray();
            string twoChars = charArray[0].ToString() + charArray[1].ToString().ToLower();

            TextMesh upSideText = handPieceGameObject.transform.Find("Front Piece Text").GetComponent<TextMesh>();
            upSideText.text = twoChars;

            upSideText.text = "";
        }

        internal void ChangePieceDownSideText(GameObject handPieceGameObject, PieceType pieceType)
        {
            char[] charArray = pieceType.ToString().Substring(0, 2).ToCharArray();
            string twoChars = charArray[0].ToString() + charArray[1].ToString().ToLower();

            if (pieceType == PieceType.COMMANDER)
            {
                twoChars = ""; // Exception to general rule
            }

            TextMesh downSideText = handPieceGameObject.transform.Find("Back Piece Text").GetComponent<TextMesh>();
            downSideText.text = twoChars;

            downSideText.text = "";
        }

        internal void ChangePieceIcon(
            GameObject handPieceGameObject, PlayerColor teamColor, PieceType pieceType, PieceType back)
        {
            string resourcesPath = pieceViewService.CalcResourcesPath(pieceType, back);
            SpriteRenderer spriteRenderer = handPieceGameObject.transform.Find("Icon").GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>(resourcesPath);

            if (teamColor == PlayerColor.WHITE)
            {
                spriteRenderer.transform.localRotation = Quaternion.Euler(180, 180, 0);
            }
        }
    }
}
