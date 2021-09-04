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
        private Sprite m_nodeSprite = null;

        [SerializeField]
        private List<ModuleNode> m_modules = new List<ModuleNode>();
        #endregion


        #region Data Structure
        [System.Serializable]
        public class ModuleNode
        {
            #region Fields
            [SerializeField]
            private GameModuleConfig m_config = null;

            [SerializeField]
            private Vector2 m_position = Vector2.zero;
            #endregion


            #region Properties
            public GameModuleConfig config => m_config;
            public Vector2 position => m_position;
            #endregion
        }
        #endregion


        #region Properties
        public Sprite worldBackground => m_worldBackground;
        public Sprite nodeSprite => m_nodeSprite;
        public int moduleCount => m_modules.Count;
        #endregion


        #region Method
        public ModuleNode GetNode(int index)
        {
            if (index < 0 || index >= moduleCount)
                return null;

            return m_modules[index];
        }
        #endregion
    }
}