using UnityEngine;

namespace archipelaGO.SceneHandling
{
    [RequireComponent(typeof(SceneLoadTrigger))]
    public abstract class SceneLinker<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        #region Field
        private SceneLoadTrigger m_sceneLoadTrigger = null;
        #endregion


        #region MonoBehaviour Implementation
        protected void Awake()
        {
            m_sceneLoadTrigger = GetComponent<SceneLoadTrigger>();

            if (m_sceneLoadTrigger != null)
                m_sceneLoadTrigger.OnSceneLoaded += InvokeSceneLoaded;
        }
        #endregion


        #region Abstract Method
        protected abstract void OnSceneLoaded(T linkable);
        #endregion


        #region Internal Methods
        private void InvokeSceneLoaded()
        {
            if (m_sceneLoadTrigger != null)
                m_sceneLoadTrigger.OnSceneLoaded -= InvokeSceneLoaded;

            T linkable = GameObject.FindObjectOfType<T>();

            if (linkable != null)
                OnSceneLoaded(linkable);
        }
        #endregion
    }
}