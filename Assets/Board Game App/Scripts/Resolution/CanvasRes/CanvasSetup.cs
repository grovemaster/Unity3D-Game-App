using UnityEngine;

namespace Resolution.CanvasRes
{
    class CanvasSetup : MonoBehaviour
    {
        private void Awake()
        {
            SetCanvasAttributes();
        }

        private void SetCanvasAttributes()
        {
#if UNITY_ANDROID
            Canvas canvas = FindObjectOfType<Canvas>();
            canvas.scaleFactor = 3.5f;
#endif
        }
    }
}
