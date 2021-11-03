using UnityEngine;
using SceneLoadTrigger = archipelaGO.SceneHandling.SceneLoadTrigger;
using Scene = archipelaGO.SceneHandling.Scene;
using WorldMapNode = archipelaGO.WorldMap.WorldMapNode;
using archipelaGO.GameData;

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
            (Vector2 position, float scale) nodeTransform = library.GetNodeTransformation(index);
            GameObject nodeGameObject = Instantiate(m_nodePrefab, nodeTransform.position, Quaternion.identity);
            nodeGameObject.name = $"Node { index + 1 }";
            nodeGameObject.transform.SetParent(transform);
            nodeGameObject.transform.localScale = (Vector3.one * nodeTransform.scale);

            (string label, Sprite sprite) nodeVisuals =
                library.GetNodeVisuals(index);

            (string[] required, string[] unlocked) keys =
                library.GetProgressKeys(index);

            bool nodeUnlocked = NodeUnlocked(keys.required);

            WorldMapNode nodeController =
                nodeGameObject.GetComponent<WorldMapNode>();

            if (nodeController != null)
                nodeController.SetVisuals(nodeVisuals.sprite, nodeVisuals.label, nodeUnlocked);

            GameModuleLinker moduleMediator = nodeGameObject.AddComponent<GameModuleLinker>();
            SceneLoadTrigger sceneLoadTrigger = nodeGameObject.GetComponent<SceneLoadTrigger>();

            moduleMediator.Initialize(library, index, keys.unlocked);
            sceneLoadTrigger.SetSceneToLoad(Scene.Game, nodeUnlocked);
        }

        private bool NodeUnlocked(string[] keys)
        {
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
        #endregion
    }
}