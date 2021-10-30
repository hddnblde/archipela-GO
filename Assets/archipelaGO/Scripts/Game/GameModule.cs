using System.Collections;
using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class GameModule<T> : MonoBehaviour, IAutoplayable
        where T : GameModuleConfig
    {
        #region Fields
        [SerializeField]
        private float m_timeLimit = Mathf.Infinity;

        private T m_config = null;
        public delegate void GameCompleted(string message);
        public event GameCompleted OnSucceeded;
        public event GameCompleted OnFailed;
        protected abstract bool objectiveComplete { get; }
        #endregion


        #region Property
        protected T config => m_config;
        #endregion


        #region Methods
        public void Initialize(T config)
        {
            m_config = config;
            OnInitialize();
        }

        protected abstract void OnInitialize();

        protected void InvokeGameOver()
        {
            bool objectiveComplete = this.objectiveComplete;

            GameCompleted completionEvent = (objectiveComplete ?
                OnSucceeded : OnFailed);

            string message = (m_config != null ?
                (objectiveComplete ? m_config.endMessage : m_config.failMessage) :
                "Game ended!");

            completionEvent?.Invoke(message);
            completionEvent = null;
        }
        #endregion


        #if ARCHIPELAGO_DEBUG_MODE
        public abstract IEnumerator Debug_Autoplay();
        #endif
    }
}