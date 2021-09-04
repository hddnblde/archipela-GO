using SceneLinker = archipelaGO.SceneHandling.SceneLinker<archipelaGO.Game.GameLoader>;
using SceneLoadTrigger = archipelaGO.SceneHandling.SceneLoadTrigger;

namespace archipelaGO.Game
{
    public class GameModuleMediator : SceneLinker
    {
        #region Fields
        private GameModuleConfig m_gameConfig = null;
        private string[] m_unlockableModules = null;
        #endregion


        #region Methods
        public void Initialize(GameModuleConfig module, SceneLoadTrigger sceneLoadTrigger, params string[] unlockableModules)
        {
            m_gameConfig = module;
            m_unlockableModules = unlockableModules;
            SetSceneLoadTrigger(sceneLoadTrigger);
        }

        protected override void OnSceneLoaded(GameLoader gameLoader)
        {
            if (gameLoader != null)
                gameLoader.LoadModule(m_gameConfig, m_unlockableModules);
        }
        #endregion
    }
}