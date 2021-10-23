using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.Game
{
    [CreateAssetMenu(fileName = "Game Library", menuName = "archipelaGO/Game Library")]
    public class GameLibrary : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private Sprite m_worldBackground = null;

        [SerializeField]
        private List<Sprite> m_nodeSprites = new List<Sprite>();

        [SerializeField]
        private List<ModuleNode> m_modules = new List<ModuleNode>();
        #endregion


        #region Data Structure
        [System.Serializable]
        public class ModuleNode
        {
            #region Fields
            [SerializeField]
            private string m_label = string.Empty;

            [SerializeField]
            private GameModuleConfig m_config = null;

            [SerializeField]
            private Vector2 m_position = Vector2.zero;

            [SerializeField]
            private int m_spriteIndex = 0;
            #endregion


            #region Properties
            public string label => m_label;
            public GameModuleConfig config => m_config;
            public Vector2 position => m_position;
            public int spriteIndex => m_spriteIndex;
            #endregion
        }
        #endregion


        #region Properties
        public Sprite worldBackground => m_worldBackground;
        public int moduleCount => m_modules.Count;
        #endregion


        #region Methods
        public Vector2 GetNodePosition(int index)
        {
            ModuleNode node = GetNode(index);

            if (node != null)
                return node.position;

            else
                return Vector2.zero;
        }

        public GameModuleConfig GetNodeConfig(int index)
        {
            ModuleNode node = GetNode(index);

            if (node != null)
                return node.config;

            else
                return null;
        }

        public (string label, Sprite sprite) GetNodeVisuals(int index)
        {
            ModuleNode node = GetNode(index);

            if (node != null)
                return (node.label, GetSprite(node.spriteIndex));

            else
                return (string.Empty, null);
        }

        private ModuleNode GetNode(int index)
        {
            if (index < 0 || index >= moduleCount)
                return null;

            return m_modules[index];
        }

        private Sprite GetSprite(int index)
        {
            if (index < 0 || index >= m_nodeSprites.Count)
                return null;

            return m_nodeSprites[index];
        }
        #endregion
    }
}