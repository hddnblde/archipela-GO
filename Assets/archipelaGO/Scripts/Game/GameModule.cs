using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class GameModule<T> : MonoBehaviour
        where T : GameConfig
    {
        #region Fields
        private T m_gameConfig = null;

        public delegate void GameCompleted();
        public event GameCompleted OnGameCompleted;
        #endregion


        #region Methods
        public abstract void Initialize(T config);

        protected void InvokeGameCompleted()
        {
            OnGameCompleted?.Invoke();
            OnGameCompleted = null;
            Debug.LogWarning($"Game Module [{ m_gameConfig.name }] completed!");
        }
        #endregion
    }
}