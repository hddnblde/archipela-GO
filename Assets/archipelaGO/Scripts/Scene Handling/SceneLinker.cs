using UnityEngine;

namespace archipelaGO.SceneHandling
{
    public abstract class SceneLinker<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        #region Field
        private SceneLoadTrigger m_sceneLoadTrigger = null;
        #endregion


        #region Abstract Method
        protected abstract void OnSceneLoaded(T linkable);
        #endregion


        #region Internal Methods
        protected void SetSceneLoadTrigger(SceneLoadTrigger sceneLoadTrigger)
        {
            m_sceneLoadTrigger = sceneLoadTrigger;

            if (m_sceneLoadTrigger != null)
                sceneLoadTrigger.OnSceneLoaded += InvokeSceneLoaded;
        }

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