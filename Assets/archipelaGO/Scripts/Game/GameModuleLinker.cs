using SceneLinker = archipelaGO.SceneHandling.SceneLinker<archipelaGO.Game.GameLoader>;
using SceneLoadTrigger = archipelaGO.SceneHandling.SceneLoadTrigger;

namespace archipelaGO.Game
{
    public class GameModuleLinker : SceneLinker
    {
        #region Fields
        private GameModuleConfig m_module = null;
        private string[] m_unlockableModules = null;
        #endregion


        #region Methods
        public void Initialize(GameModuleConfig module, SceneLoadTrigger sceneLoadTrigger, params string[] unlockableModules)
        {
            m_module = module;
            m_unlockableModules = unlockableModules;
            SetSceneLoadTrigger(sceneLoadTrigger);
        }

        protected override void OnSceneLoaded(GameLoader gameLoader)
        {
            if (gameLoader != null)
                gameLoader.LoadModule(m_module, m_unlockableModules);
        }
        #endregion
    }
}