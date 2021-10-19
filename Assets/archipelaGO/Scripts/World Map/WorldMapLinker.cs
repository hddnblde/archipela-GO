using archipelaGO.Game;
using UnityEngine;
using SceneLinker = archipelaGO.SceneHandling.SceneLinker<archipelaGO.Game.GameWorldManager>;

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

        public void SetGameLibrary(GameLibrary library) =>
            m_library = library;

        protected override void OnSceneLoaded(GameWorldManager linkable)
        {
            if (linkable != null)
                linkable.LoadWorld(m_library);
        }
    }
}