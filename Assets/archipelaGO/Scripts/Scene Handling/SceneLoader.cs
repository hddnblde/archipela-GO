using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace archipelaGO.SceneHandling
{
    public class SceneLoader : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float m_minLoadingTime = 3f;

        [SerializeField]
        private float m_fadeDuration = 1.25f;

        [SerializeField]
        private CanvasGroup m_screenBlocker = null;

        private Coroutine m_loadSceneRoutine = null;
        private bool m_isTransitioning = false, m_loaded = false;
        public delegate void SceneLoaded(Scene scene);
        public static event SceneLoaded OnPreLoad;
        public static event SceneLoaded OnPostLoad;
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

        public static bool IsActive() =>
            (m_singletonInstance != null);

        public static bool NotTransitioning()
        {
            if (!IsActive())
                return true;
            
            return !m_singletonInstance.m_isTransitioning;
        }

        public static bool FinishedLoadingScene()
        {
            if (IsActive())
                return m_singletonInstance.m_loaded;

            return true;
        }

        public static bool CanLoadANewScene()
        {
            if (!IsActive())
                return true;

            return !m_singletonInstance.m_isTransitioning &&
                m_singletonInstance.m_loaded;
        }

        public static void LoadScene(Scene scene)
        {
            if (IsActive())
                m_singletonInstance.InternalLoadScene(scene);
        }

        public static void LoadScene(int sceneIndex)
        {
            if (IsActive())
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
            Scene scene = (Scene)sceneIndex;
            m_isTransitioning = true;
            m_loaded = false;

            OnPreLoad?.Invoke(scene);
            yield return FadeScreenRoutine(true);
            yield return LoadSceneAsynchronously(sceneIndex);
            yield return FadeScreenRoutine(false);
            m_isTransitioning = false;
            OnPostLoad?.Invoke(scene);
        }

        private IEnumerator LoadSceneAsynchronously(int sceneIndex)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            AsyncOperation sceneLoading =
                SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

            float startTime = Time.time;

            yield return new WaitUntil(() => sceneLoading.isDone);

            m_loaded = true;

            AsyncOperation sceneUnloading =
                SceneManager.UnloadSceneAsync(currentSceneIndex);

            yield return new WaitUntil(() => sceneUnloading.isDone);
            yield return WaitForLoadTimeToFinish(startTime);
        }

        private IEnumerator WaitForLoadTimeToFinish(float startTime)
        {
            float totalLoadingTime = (Time.time - startTime);
            float remainingLoadTime = Mathf.Max(m_minLoadingTime - totalLoadingTime, 0f);

            if (remainingLoadTime > 0f)
                yield return new WaitForSeconds(remainingLoadTime);
        }

        private IEnumerator FadeScreenRoutine(bool fadeIn)
        {
            if (fadeIn)
                BlockRaycasts(true);

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
                BlockRaycasts(false);
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