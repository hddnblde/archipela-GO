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
        public event GameCompleted OnGameCompleted;
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

        protected void InvokeGameCompleted()
        {
            OnGameCompleted?.Invoke(m_config?.endMessage ?? "Game ended!");
            OnGameCompleted = null;
        }
        #endregion


        #if ARCHIPELAGO_DEBUG_MODE
        public abstract IEnumerator Debug_Autoplay();
        #endif
    }
}