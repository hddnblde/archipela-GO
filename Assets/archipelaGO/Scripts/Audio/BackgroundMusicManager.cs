using System.Collections.Generic;
using UnityEngine;
using archipelaGO.SceneHandling;
using archipelaGO.Game;

namespace archipelaGO.Audio
{
    public class BackgroundMusicManager : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private BackgroundMusicPlayer m_player = null;

        [SerializeField]
        private List<MainBGM> m_mainBGMList = new List<MainBGM>();

        [SerializeField]
        private List<GameBGM> m_gameBGMList = new List<GameBGM>();

        private static BackgroundMusicManager m_manager = null;
        #endregion


        #region Data Structure
        [System.Serializable]
        private struct MainBGM
        {
            [SerializeField]
            private Scene m_scene;

            [SerializeField]
            private AudioClip m_clip;

            public bool Matched(Scene scene, out AudioClip clip)
            {
                bool matched = (m_scene == scene);
                clip = (matched ? m_clip : null);

                return matched;
            }
        }

        [System.Serializable]
        private struct GameBGM
        {
            [SerializeField]
            private GameModuleConfig m_config;

            [SerializeField]
            private AudioClip m_clip;

            public bool Matched(GameModuleConfig config, out AudioClip clip)
            {
                bool matched = (m_config == config);
                clip = (matched ? m_clip : null);

                return matched;
            }
        }
        #endregion


        #region MonoBehaviour Implementation
        private void Awake()
        {
            if (m_manager != null)
            {
                Destroy(gameObject);
                return;
            }

            m_manager = this;
            SceneLoader.OnPreLoad += OnScenePreLoad;
            GameLoader.OnGameLoaded += OnGameLoaded;
        }
        private void OnDestroy()
        {
            if (m_manager == this)
                m_manager = null;

            SceneLoader.OnPreLoad -= OnScenePreLoad;
            GameLoader.OnGameLoaded -= OnGameLoaded;
        }
        #endregion


        #region Internal Methods
        private void OnScenePreLoad(Scene scene)
        {
            if (scene == Scene.Game)
                return;

            AudioClip music = GetAudioClipForScene(scene);

            if (music != null && m_player != null)
                m_player.Play(music);
        }

        private void OnGameLoaded(GameModuleConfig gameConfig)
        {
            AudioClip music = GetAudioClipForGame(gameConfig);

            if (music != null && m_player != null)
                m_player.Play(music);
        }

        private AudioClip GetAudioClipForScene(Scene scene)
        {
            AudioClip clip = null;

            foreach (MainBGM bgm in m_mainBGMList)
            {
                if (bgm.Matched(scene, out clip))
                    break;
            }

            return clip;
        }

        private AudioClip GetAudioClipForGame(GameModuleConfig gameConfig)
        {
            AudioClip clip = null;

            foreach (GameBGM bgm in m_gameBGMList)
            {
                if (bgm.Matched(gameConfig, out clip))
                    break;
            }

            return clip;
        }
        #endregion
    }
}