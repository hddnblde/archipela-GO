using System.Collections;
using UnityEngine;
using ProgressManager = archipelaGO.Progression.ProgressManager;

namespace archipelaGO.SceneHandling
{
    public class SceneLoadTrigger : ClickableObject
    {
        [SerializeField]
        private ProgressManager m_progressManager = null;

        [SerializeField]
        private Scene m_sceneToLoad = Scene.Boot;

        private Coroutine m_loadRoutine = null;

        protected override bool IsInteractable()
        {
            if (m_progressManager == null)
                return true;

            return m_progressManager.IsUnlocked();
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

        private IEnumerator LoadSceneRoutine()
        {
            yield return new WaitUntil(SceneLoader.CanLoadANewScene);
            SceneLoader.LoadScene(m_sceneToLoad);
        }
    }
}