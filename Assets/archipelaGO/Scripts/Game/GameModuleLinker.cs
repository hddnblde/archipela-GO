using SceneLinker = archipelaGO.SceneHandling.SceneLinker<archipelaGO.Game.GameLoader>;

namespace archipelaGO.Game
{
    public class GameModuleLinker : SceneLinker
    {
        #region Fields
        private GameLibrary m_library = null;
        private int m_moduleIndex = -1;
        private string[] m_unlockableModules = null;
        #endregion


        #region Methods
        public void Initialize(GameLibrary library, int moduleIndex, params string[] unlockableModules)
        {
            m_library = library;
            m_moduleIndex = moduleIndex;
            m_unlockableModules = unlockableModules;
        }

        protected override void OnSceneLoaded(GameLoader gameLoader)
        {
            if (gameLoader == null || m_library == null ||
                m_moduleIndex < 0 || m_moduleIndex >= m_library.moduleCount)
                return;

            gameLoader.SetUpWorldMapLinkers(m_library);

            GameModuleConfig moduleConfig =
                m_library.GetNodeConfig(m_moduleIndex);

            if (moduleConfig != null)
                gameLoader.LoadModule(moduleConfig, m_unlockableModules);
        }
        #endregion
    }
}