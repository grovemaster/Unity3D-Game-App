using Data.Enums.Piece;
using UnityEngine;

namespace View.Hand
{
    public class HandViewService
    {
        public void ChangePieceUpSideText(GameObject handPieceGameObject, PieceType pieceType)
        {
            char[] charArray = pieceType.ToString().Substring(0, 2).ToCharArray();
            string twoChars = charArray[0].ToString() + charArray[1].ToString().ToLower();

            TextMesh upSideText = handPieceGameObject.transform.Find("Front Piece Text").GetComponent<TextMesh>();
            upSideText.text = twoChars;
        }

        internal void ChangePieceDownSideText(GameObject handPieceGameObject, PieceType pieceType)
        {
            char[] charArray = pieceType.ToString().Substring(0, 2).ToCharArray();
            string twoChars = charArray[0].ToString() + charArray[1].ToString().ToLower();

            if (pieceType == PieceType.COMMANDER)
            {
                twoChars = ""; // Exception to general rule
            }

            TextMesh upSideText = handPieceGameObject.transform.Find("Back Piece Text").GetComponent<TextMesh>();
            upSideText.text = twoChars;
        }
    }
}
