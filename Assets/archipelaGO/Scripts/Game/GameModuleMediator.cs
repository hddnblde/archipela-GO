using System.Collections.Generic;
using UnityEngine;
using SceneLoadTrigger = archipelaGO.SceneHandling.SceneLoadTrigger;

namespace archipelaGO.Game
{
    public class GameModuleMediator : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private SceneLoadTrigger m_sceneLoadTrigger = null;

        [SerializeField]
        private GameConfig m_gameConfig = null;

        [SerializeField]
        private List<GameConfig> m_unlockableModules = new List<GameConfig>();
        #endregion


        #region MonoBehaviour Implementation
        private void Awake()
        {
            if (m_sceneLoadTrigger != null)
                m_sceneLoadTrigger.OnSceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (m_sceneLoadTrigger != null)
                m_sceneLoadTrigger.OnSceneLoaded -= OnSceneLoaded;
        }
        #endregion


        #region Internal Methods
        private void OnSceneLoaded()
        {
            GameModuleManager moduleManager =
                GameObject.FindObjectOfType<GameModuleManager>();

            if (moduleManager != null)
                moduleManager.LoadModule(m_gameConfig, GetUnlockableModules());
        }

        private string[] GetUnlockableModules()
        {
            List<string> unlockableKeys = new List<string>();

            foreach (GameConfig unlockableModule in m_unlockableModules)
            {
                if (unlockableModule != null)
                    unlockableKeys.Add(unlockableModule.name);
            }

            return unlockableKeys.ToArray();
        }
        #endregion
    }
}