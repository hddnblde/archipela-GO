using System.Collections.Generic;
using archipelaGO.Game;
using archipelaGO.GameData;
using UnityEngine;
using SceneLinker = archipelaGO.SceneHandling.SceneLinker<archipelaGO.Game.GameWorldManager>;
using GameModuleConfig = archipelaGO.Game.GameModuleConfig;

namespace archipelaGO.WorldMap
{
    public interface IWorldMapLinker
    {
        void SetGameLibrary(GameLibrary library);
    }

    public class WorldMapLinker : SceneLinker, IWorldMapLinker
    {
        [SerializeField]
        private GameLibrary m_library = null;

        [SerializeField]
        private bool m_preUnlocked = false;

        [SerializeField]
        private CanvasGroup m_canvasGroup = null;

        [SerializeField]
        private List<GameObject> m_interactables = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();
            bool unlocked = (m_preUnlocked || IsUnlocked());

            if (sceneLoadTrigger != null)
                sceneLoadTrigger.isInteractable = unlocked;

            if (m_canvasGroup != null)
            {
                m_canvasGroup.interactable = unlocked;
                m_canvasGroup.blocksRaycasts = unlocked;
            }

            foreach (GameObject interactable in m_interactables)
            {
                if (interactable != null)
                    interactable.SetActive(unlocked);
            }
        }

        public void SetGameLibrary(GameLibrary library) =>
            m_library = library;

        protected override void OnSceneLoaded(GameWorldManager linkable)
        {
            if (linkable != null)
                linkable.LoadWorld(m_library);
        }

        private bool IsUnlocked()
        {
            string[] keys = GetRequiredKeys();

            if (keys == null)
                return false;

            else if (keys.Length <= 0)
                return true;

            PlayerData playerData = GameDataHandler.CurrentPlayer();

            if (playerData == null)
            {
                Debug.LogError("Cannot determine if node is unlocked because no player data was loaded!");
                return false;
            }

            GameProgressionData progressData = playerData.
                Access<GameProgressionData>();

            if (progressData == null)
            {
                Debug.LogError("Cannot determine if node is unlocked because there is no progress data.");
                return false;
            }

            return progressData.AreUnlocked(keys);
        }

        private string[] GetRequiredKeys()
        {
            if (m_library == null)
                return null;

            List<string> keys = new List<string>();

            for (int i = 0; i < m_library.moduleCount; i++)
            {
                GameModuleConfig config = m_library.GetNodeConfig(i);

                if (config != null)
                    keys.Add(config.name);
            }

            return keys.ToArray();
        }
    }
}