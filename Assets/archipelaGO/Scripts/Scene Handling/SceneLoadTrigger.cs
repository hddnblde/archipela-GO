using System.Collections;
using UnityEngine;

namespace archipelaGO.SceneHandling
{
    public class SceneLoadTrigger : MonoBehaviour
    {
        [SerializeField]
        private Scene m_sceneToLoad = Scene.Boot;

        private Coroutine m_loadRoutine = null;

        public void TriggerSceneLoader()
        {
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