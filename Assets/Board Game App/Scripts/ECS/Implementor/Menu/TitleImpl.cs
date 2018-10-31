using ECS.Component.Menu;
using Svelto.ECS;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ECS.Implementor.Menu
{
    class TitleImpl : MonoBehaviour, IImplementor, ITitleComponent
    {
        public DispatchOnSet<bool> Clicked { get; set; }
        public Action ClickAction { get; set; }

        void Awake()
        {
            Clicked = new DispatchOnSet<bool>
            {
                value = false
            };

            ClickAction = LoadTitleScene;
        }

        public void GotoTitleScreen()
        {
            Clicked.value = true;
        }

        private void LoadTitleScene()
        {
            SceneManager.LoadScene("Title");
        }
    }
}
