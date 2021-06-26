using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace archipelaGO.SceneHandling
{
    public class SceneLoader : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float m_fadeDuration = 1.25f;

        [SerializeField]
        private CanvasGroup m_screenBlocker = null;

        private Coroutine m_loadSceneRoutine = null;
        private bool m_isTransitioning = false;
        private static SceneLoader m_singletonInstance = null;
        #endregion


        #region MonoBehaviour Implementation
        private void Awake() => InitializeSingleton();
        private void OnDestroy() => UninitializeSingleton();
        #endregion


        #region Singleton Implementation
        private void InitializeSingleton()
        {
            if (m_singletonInstance == null)
                m_singletonInstance = this;

            else
                Destroy(gameObject);
        }

        private void UninitializeSingleton()
        {
            if (m_singletonInstance == this)
                m_singletonInstance = null;
        }

        public static bool IsTransitioning()
        {
            if (m_singletonInstance != null)
                return m_singletonInstance.m_isTransitioning;

            return false;
        }

        public static void LoadScene(Scene scene)
        {
            if (m_singletonInstance != null)
                m_singletonInstance.InternalLoadScene(scene);
        }

        public static void LoadScene(int sceneIndex)
        {
            if (m_singletonInstance != null)
                m_singletonInstance.InternalLoadScene(sceneIndex);
        }
        #endregion


        #region Methods
        private void InternalLoadScene(Scene scene) =>
            InternalLoadScene((int)scene);

        private void InternalLoadScene(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogWarning($"Failed to load scene index [{ sceneIndex }] because it is out of bounds!");
                return;
            }

            if (m_isTransitioning)
            {
                Debug.LogWarning("Please wait until scene transition is finished before loading a new scene!");
                return;
            }

            if (m_loadSceneRoutine != null)
                StopCoroutine(m_loadSceneRoutine);
            
            m_loadSceneRoutine = StartCoroutine(SceneLoadingRoutine(sceneIndex));
        }

        private IEnumerator SceneLoadingRoutine(int sceneIndex)
        {
            m_isTransitioning = true;
            yield return FadeScreenRoutine(true);
            yield return LoadSceneAsynchronously(sceneIndex);
            yield return FadeScreenRoutine(false);
            m_isTransitioning = false;
        }

        private IEnumerator LoadSceneAsynchronously(int sceneIndex)
        {
            AsyncOperation sceneLoading =
                SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

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
        #endregion
    }
}