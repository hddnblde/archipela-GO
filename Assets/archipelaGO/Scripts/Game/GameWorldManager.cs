using UnityEngine;
using GameModuleNode = archipelaGO.Game.GameLibrary.ModuleNode;
using SceneLoadTrigger = archipelaGO.SceneHandling.SceneLoadTrigger;
using Scene = archipelaGO.SceneHandling.Scene;

namespace archipelaGO.Game
{
    public class GameWorldManager : MonoBehaviour
    {
        #region Field
        [SerializeField]
        private GameObject m_nodePrefab = null;

        [SerializeField]
        private SpriteRenderer m_worldBackgroundRenderer = null;
        #endregion


        #region Methods
        public void LoadWorld(GameLibrary library)
        {
            if (library == null)
            {
                Debug.LogError("Failed to create nodes. Please assign a library then try again!");
                return;
            }

            InitializeWorldBackground(library);
            CreateNodes(library);
        }

        private void InitializeWorldBackground(GameLibrary library)
        {
            if (m_worldBackgroundRenderer != null)
                m_worldBackgroundRenderer.sprite = library.worldBackground;
        }

        private void CreateNodes(GameLibrary library)
        {
            if (m_nodePrefab == null)
            {
                Debug.LogError("Failed to create nodes. Please assign a node prefab then try again!");
                return;
            }

            for (int i = 0; i < library.moduleCount; i++)
                CreateNode(library, i);
        }

        private void CreateNode(GameLibrary library, int index)
        {
            Vector2 nodePosition = library.GetNodePosition(index);
            GameObject nodeGameObject = Instantiate(m_nodePrefab, nodePosition, Quaternion.identity);
            nodeGameObject.name = $"Node { index + 1 }";
            nodeGameObject.transform.SetParent(transform);

            GameModuleLinker moduleMediator = nodeGameObject.AddComponent<GameModuleLinker>();
            SceneLoadTrigger sceneLoadTrigger = nodeGameObject.GetComponent<SceneLoadTrigger>();

            moduleMediator.Initialize(library, index);
            sceneLoadTrigger.SetSceneToLoad(Scene.Game);
        }
        #endregion
    }
}