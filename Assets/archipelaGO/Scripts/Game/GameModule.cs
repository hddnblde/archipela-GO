using System.Collections;
using UnityEngine;

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

        public event Tick OnCountUp;
        public event Tick OnCountdown;
        public delegate void Tick(int seconds);
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
        public void Initialize(T config)
        {
            m_config = config;
            OnInitialize();

            if (timeLimit < Mathf.Infinity)
                BeginCountdownTimer(timeLimit);
        }

        protected abstract void OnInitialize();

        protected void InvokeGameOver()
        {
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

        private void BeginCountdownTimer(float timeLimit)
        {
            if (m_timerRoutine != null)
                StopCoroutine(m_timerRoutine);

            m_timerRoutine = StartCoroutine(CountdownRoutine(timeLimit));
        }

        private IEnumerator CountdownRoutine(float timeLimit)
        {
            int previousTick = -1;

            for (m_currentTime = 0f; m_currentTime < timeLimit; m_currentTime += Time.deltaTime)
            {
                int flooredTime = Mathf.FloorToInt(m_currentTime);

                if (previousTick != flooredTime)
                {
                    previousTick = flooredTime;
                    InvokeTick();
                }
                yield return null;
            }

            InvokeTick();
            yield return null;
            InvokeGameOver();
        }

        private void InvokeTick()
        {
            OnCountUp?.Invoke(Mathf.FloorToInt(currentTime));
            OnCountdown?.Invoke(Mathf.FloorToInt(remainingTime));
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