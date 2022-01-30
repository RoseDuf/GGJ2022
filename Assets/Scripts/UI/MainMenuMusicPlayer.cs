using System;
using Game;
using UnityEngine;

namespace UI
{
    public class MainMenuMusicPlayer : MonoBehaviour
    {
        private void Start()
        {
            SoundSystem.Instance.PlayMenuMusic();
        }
    }
}