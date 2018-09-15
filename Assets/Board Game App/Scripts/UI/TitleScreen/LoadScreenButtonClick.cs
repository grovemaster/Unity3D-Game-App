using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.TitleScreen
{
    public class LoadScreenButtonClick : MonoBehaviour
    {
        public void LoadNewGameScene()
        {
            SceneManager.LoadScene("BoardGame");
        }

        public void LoadRulesScene()
        {
            SceneManager.LoadScene("Rules");
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
