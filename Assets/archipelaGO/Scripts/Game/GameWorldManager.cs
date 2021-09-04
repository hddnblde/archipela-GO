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
        private GameLibrary m_library = null;

        [SerializeField]
        private GameObject m_nodePrefab = null;

        [SerializeField]
        private SpriteRenderer m_worldBackgroundRenderer = null;
        #endregion


        #region MonoBehaviour Implementation
        private void Awake() => InitializeWorldMap();
        #endregion


        #region Internal Methods
        private void InitializeWorldMap()
        {
            if (m_library == null)
            {
                Debug.LogError("Failed to create nodes. Please assign a library then try again!");
                return;
            }

            InitializeWorldBackground();
            CreateNodes();
        }

        private void InitializeWorldBackground()
        {
            if (m_worldBackgroundRenderer != null)
                m_worldBackgroundRenderer.sprite = m_library.worldBackground;
        }

        private void CreateNodes()
        {
            if (m_nodePrefab == null)
            {
                Debug.LogError("Failed to create nodes. Please assign a node prefab then try again!");
                return;
            }

            for (int i = 0; i < m_library.moduleCount; i++)
            {
                GameModuleNode moduleNode = m_library.GetNode(i);
                CreateNode(i, moduleNode);
            }
        }

        private void CreateNode(int index, GameModuleNode node)
        {
            GameObject nodeGameObject = Instantiate(m_nodePrefab, node.position, Quaternion.identity);
            nodeGameObject.name = $"Node { index + 1 }";
            nodeGameObject.SetActive(false);
            nodeGameObject.transform.SetParent(transform);

            GameModuleLinker moduleMediator = nodeGameObject.AddComponent<GameModuleLinker>();
            SceneLoadTrigger sceneLoadTrigger = nodeGameObject.AddComponent<SceneLoadTrigger>();

            moduleMediator.Initialize(node.config, sceneLoadTrigger);
            sceneLoadTrigger.SetSceneToLoad(Scene.Game);
            nodeGameObject.SetActive(true);
        }

        #endregion
    }
}