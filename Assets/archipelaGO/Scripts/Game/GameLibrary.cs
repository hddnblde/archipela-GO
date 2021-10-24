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
        private GameProgressCollection m_progressKeys = null;

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

            [SerializeField]
            private List<GameModuleConfig> m_requiredKeys = new List<GameModuleConfig>();

            [SerializeField]
            private List<GameModuleConfig> m_unlockedKeys = new List<GameModuleConfig>();
            #endregion


            #region Properties
            public string label => m_label;
            public GameModuleConfig config => m_config;
            public Vector2 position => m_position;
            public int spriteIndex => m_spriteIndex;
            #endregion


            #region Methods
            public int[] GetRequiredKeys(GameProgressCollection keyList) =>
                GetKeysIndices(keyList, m_requiredKeys);

            public int[] GetUnlockedKeys(GameProgressCollection keyList) =>
                GetKeysIndices(keyList, m_unlockedKeys);
            
            private int[] GetKeysIndices(GameProgressCollection keyList, List<GameModuleConfig> keys)
            {
                if (keyList == null)
                    return new int[0];

                List<int> keyIndices = new List<int>();

                foreach (GameModuleConfig key in keys)
                {
                    int keyIndex = keyList.IndexOf(key);
                    keyIndices.Add(keyIndex);
                }

                return keyIndices.ToArray();
            }
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

        public (string[] requirements, string[] unlockables) GetProgressKeys(int index)
        {
            ModuleNode node = GetNode(index);

            if (node == null || m_progressKeys == null)
                return (null, null);

            return (GetProgressKeyChain(node.GetRequiredKeys(m_progressKeys)),
                GetProgressKeyChain(node.GetUnlockedKeys(m_progressKeys)));
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

        private string[] GetProgressKeyChain(int[] keys)
        {
            if (keys == null || keys.Length <= 0 ||
                m_progressKeys == null || m_progressKeys.keyCount <= 0)
                return null;

            string[] keyNames = new string[keys.Length];

            for (int i = 0; i < keyNames.Length; i++)
                keyNames[i] = m_progressKeys.GetKey(i);

            return keyNames;
        }
        #endregion
    }
}