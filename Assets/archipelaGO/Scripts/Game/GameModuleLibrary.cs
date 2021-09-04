using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.Game
{
    [CreateAssetMenu(fileName = "Game Module Library", menuName = "archipelaGO/Game Module Library")]
    public class GameConfigLibrary : ScriptableObject
    {
        #region Field
        [SerializeField]
        private List<ModuleNode> m_modules = new List<ModuleNode>();
        #endregion


        #region Data Structure
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


        #region Property
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