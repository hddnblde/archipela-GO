using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class GameModule<T> : MonoBehaviour
        where T : GameConfig
    {
        #region Fields
        private T m_gameConfig = null;

        public delegate void GameCompleted(T gameConfig);
        public event GameCompleted OnGameCompleted;
        #endregion


        #region Methods
        public void Initialize(T config)
        {
            m_gameConfig = config;
            OnInitialize(config);
        }

        protected abstract void OnInitialize(T config);

        protected void InvokeGameCompleted()
        {
            OnGameCompleted?.Invoke(m_gameConfig);
            Debug.LogWarning($"Game Module ({ m_gameConfig.name }) completed!");
        }
        #endregion
    }
}