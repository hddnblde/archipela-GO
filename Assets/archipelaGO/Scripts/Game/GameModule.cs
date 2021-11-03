using System.Collections;
using UnityEngine;
using Timer = archipelaGO.TimeHandling.Timer;

namespace archipelaGO.Game
{
    public abstract class GameModule<T> : MonoBehaviour, IAutoplayable
        where T : GameModuleConfig
    {
        #region Fields
        private T m_config = null;
        public delegate void GameCompleted(string message);
        public event GameCompleted OnSucceeded;
        public event GameCompleted OnFailed;
        private Coroutine m_timerRoutine = null;
        private float m_currentTime = 0f;
        private Timer m_timer = null;
        #endregion


        #region Properties
        protected T config => m_config;
        protected abstract bool objectiveComplete { get; }

        public float timeLimit
        {
            get
            {
                if (config == null)
                    return Mathf.Infinity;

                return config.timeLimit;
            }
        }

        public float currentTime => m_currentTime;
        public float remainingTime => (timeLimit - currentTime);
        #endregion


        #region Methods
        public void Initialize(T config, Timer timer)
        {
            m_config = config;
            m_timer = timer;
            OnInitialize();
        }

        public void Begin()
        {
            if (timeLimit < Mathf.Infinity && m_timer != null)
                m_timer.StartCountdown(timeLimit, InvokeGameOver);
        }

        protected abstract void OnInitialize();

        protected void InvokeGameOver()
        {
            if (m_timer != null)
                m_timer.StopCountdown();

            #if ARCHIPELAGO_DEBUG_MODE
            StopAllAutoplayers();
            #endif

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

        private void StopAllAutoplayers()
        {
            GameModuleAutoplayer[] autoplayers =
                GameObject.FindObjectsOfType<GameModuleAutoplayer>();

            for (int i = autoplayers.Length - 1; i >= 0; i--)
            {
                GameModuleAutoplayer autoplayer = autoplayers[i];
                autoplayer.Stop();
                Destroy(autoplayer.gameObject);
            }
        }
        #endif
    }
}