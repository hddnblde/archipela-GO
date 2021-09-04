using System.Collections;
using UnityEngine;
using ProgressManager = archipelaGO.Game.ProgressManager;

namespace archipelaGO.SceneHandling
{
    public class SceneLoadTrigger : ClickableObject
    {
        #region Fields
        // private ProgressManager m_progressManager = null;

        [SerializeField]
        private Scene m_sceneToLoad = Scene.Boot;

        private Coroutine m_loadRoutine = null;
        public delegate void SceneLoaded();
        public event SceneLoaded OnSceneLoaded;
        #endregion


        #region ClickableObject Implementation
        protected override bool IsInteractable()
        {
            // if (m_progressManager == null)
            //     return true;

            // return m_progressManager.IsUnlocked();

            return true;
        }

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
        public void SetSceneToLoad(Scene scene) => m_sceneToLoad = scene;

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