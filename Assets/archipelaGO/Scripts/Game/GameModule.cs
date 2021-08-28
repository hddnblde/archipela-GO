using UnityEngine;

namespace archipelaGO.Game
{
    public delegate void GameCompleted(GameConfig gameConfig);

    public abstract class GameModule<T> : MonoBehaviour
        where T : GameConfig
    {
        #region Fields
        private T m_gameConfig = null;

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
            OnGameCompleted = null;
            Debug.LogWarning($"Game Module [{ m_gameConfig.name }] completed!");
        }
        #endregion
    }
}