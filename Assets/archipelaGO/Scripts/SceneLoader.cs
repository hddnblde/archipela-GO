using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace archipelaGO
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private float m_fadeDuration = 1.25f;

        [SerializeField]
        private CanvasGroup m_screenBlocker = null;

        private Coroutine m_loadSceneRoutine = null;
        private bool m_isTransitioning = false;

        public void LoadScene(string sceneName)
        {
            if (m_isTransitioning)
                return;
            
            if (m_loadSceneRoutine != null)
                StopCoroutine(m_loadSceneRoutine);
            
            m_loadSceneRoutine = StartCoroutine(SceneLoadingRoutine(sceneName));
        }

        private IEnumerator SceneLoadingRoutine(string sceneName)
        {
            m_isTransitioning = true;
            yield return FadeScreenRoutine(true);
            yield return LoadSceneAsynchronously(sceneName);
            yield return FadeScreenRoutine(false);
            m_isTransitioning = false;
        }

        private IEnumerator LoadSceneAsynchronously(string sceneName)
        {
            AsyncOperation sceneLoading =
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            yield return new WaitUntil(() => sceneLoading.isDone);
        }

        private IEnumerator FadeScreenRoutine(bool fadeIn)
        {
            if (fadeIn)
                BlockRaycasts(false);
            
            float startAlpha = (fadeIn ? 0f : 1f);
            float endAlpha = (fadeIn ? 1f : 0f);

            for (float current = 0f; current < m_fadeDuration; current += Time.deltaTime)
            {
                float t = Mathf.InverseLerp(0f, m_fadeDuration, current);
                float alpha = Mathf.SmoothStep(startAlpha, endAlpha, t);
                SetScreenAlpha(alpha);
                yield return null;
            }

            SetScreenAlpha(endAlpha);

            if (!fadeIn)
                BlockRaycasts(true);
            
            m_isTransitioning = false;
        }

        private void SetScreenAlpha(float alpha)
        {
            if (m_screenBlocker != null)
                m_screenBlocker.alpha = alpha;
        }

        private void BlockRaycasts(bool blockRaycast)
        {
            if (m_screenBlocker == null)
                return;

            m_screenBlocker.blocksRaycasts = blockRaycast;
            m_screenBlocker.interactable = blockRaycast;
        }
    }
}