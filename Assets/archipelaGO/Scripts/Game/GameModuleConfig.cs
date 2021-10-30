using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class GameModuleConfig : ScriptableObject
    {
        [SerializeField]
        private float m_timeLimit = Mathf.Infinity;

        [SerializeField]
        private string m_hint = string.Empty;

        [SerializeField]
        private string m_endMessage = string.Empty;

        [SerializeField]
        private string m_failMessage = "Game over.";

        [SerializeField]
        private bool m_requireTapToContinue = false;

        public float timeLimit => m_timeLimit;
        public string hint => m_hint;
        public string endMessage => m_endMessage;
        public string failMessage => m_failMessage;
        public bool requireTapToContinue => m_requireTapToContinue;
    }
}