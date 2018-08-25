using Data.Enum.Piece;
using UnityEngine;

namespace View.Piece
{
    public class PieceViewService
    {
        public void ChangePieceUpSideText(GameObject pieceGameObject, PieceType pieceType)
        {
            char[] charArray = pieceType.ToString().Substring(0, 2).ToCharArray();
            string twoChars = charArray[0].ToString() + charArray[1].ToString().ToLower();

            TextMesh upSideText = pieceGameObject.transform.Find("Up Side Text").GetComponent<TextMesh>();
            upSideText.text = twoChars;
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
        }
    }
}
