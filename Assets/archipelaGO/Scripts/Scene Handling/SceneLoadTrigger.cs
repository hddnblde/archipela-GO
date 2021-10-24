using System.Collections;
using UnityEngine;

namespace archipelaGO.SceneHandling
{
    public class SceneLoadTrigger : ClickableObject
    {
        #region Fields
        [SerializeField]
        private Scene m_sceneToLoad = Scene.Boot;

        private Coroutine m_loadRoutine = null;
        private bool m_isInteractable = true;
        public delegate void SceneLoaded();
        public event SceneLoaded OnSceneLoaded;
        #endregion


        #region ClickableObject Implementation
        protected sealed override bool IsInteractable() =>
            m_isInteractable;

        protected override void OnObjectClicked()
        {
            if (!SceneLoader.IsActive())
            {
                Debug.LogWarning("SceneLoader is not yet active. Failed to load a new scene.");
                return;
            }

            if (m_loadRoutine != null)
                StopCoroutine(m_loadRoutine);

            m_loadRoutine = StartCoroutine(LoadSceneRoutine());
        }
        #endregion


        #region Methods
        public void SetSceneToLoad(Scene scene, bool unlocked)
        {
            m_sceneToLoad = scene;
            m_isInteractable = unlocked;
        }

        private IEnumerator LoadSceneRoutine()
        {
            yield return new WaitUntil(SceneLoader.CanLoadANewScene);
            SceneLoader.LoadScene(m_sceneToLoad);
            yield return new WaitUntil(SceneLoader.FinishedLoadingScene);
            InvokeOnSceneLoaded();
        }

        private void InvokeOnSceneLoaded() =>
            OnSceneLoaded?.Invoke();
        #endregion
    }
}