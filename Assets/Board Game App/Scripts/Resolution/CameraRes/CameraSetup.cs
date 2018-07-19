using UnityEngine;

namespace Resolution.CameraRes
{
    class CameraSetup : MonoBehaviour
    {
        private void Awake()
        {
            SetCameraAttributes();
        }

        private void SetCameraAttributes()
        {
            #if UNITY_ANDROID
            Camera camera = Camera.main;
            camera.orthographicSize = 80f;
            camera.transform.position = new Vector3(
                30f,
                camera.transform.position.y,
                camera.transform.position.z
                );
            #endif
        }
    }
}
