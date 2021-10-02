using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class GameModuleConfig : ScriptableObject
    {
        [SerializeField]
        private string m_hint = string.Empty;

        [SerializeField]
        private bool m_requireTapToContinue = false;

        public string hint => m_hint;
        public bool requireTapToContinue => m_requireTapToContinue;
    }
}