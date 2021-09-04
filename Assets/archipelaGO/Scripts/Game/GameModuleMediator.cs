using UnityEngine;
using SceneLoadTrigger = archipelaGO.SceneHandling.SceneLoadTrigger;

namespace archipelaGO.Game
{
    public class GameModuleMediator : MonoBehaviour
    {
        #region Fields
        private SceneLoadTrigger m_sceneLoadTrigger = null;
        private GameModuleConfig m_gameConfig = null;
        private string[] m_unlockableModules = null;
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


        #region Methods
        public void Initialize(GameModuleConfig module, SceneLoadTrigger sceneLoadTrigger, params string[] unlockableModules)
        {
            m_gameConfig = module;
            m_sceneLoadTrigger = sceneLoadTrigger;
            m_unlockableModules = unlockableModules;
        }

        private void OnSceneLoaded()
        {
            GameLoader moduleManager =
                GameObject.FindObjectOfType<GameLoader>();

            if (moduleManager != null)
                moduleManager.LoadModule(m_gameConfig, m_unlockableModules);
        }
        #endregion
    }
}