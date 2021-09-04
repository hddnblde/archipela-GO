using UnityEngine;

namespace archipelaGO.Game
{
    public abstract class GameModule<T> : MonoBehaviour
        where T : GameModuleConfig
    {
        #region Fields
        public delegate void GameCompleted();
        public event GameCompleted OnGameCompleted;
        #endregion


        #region Methods
        public abstract void Initialize(T config);

        protected void InvokeGameCompleted()
        {
            OnGameCompleted?.Invoke();
            OnGameCompleted = null;
        }
        #endregion
    }
}