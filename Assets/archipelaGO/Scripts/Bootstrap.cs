using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using archipelaGO.SceneHandling;

namespace archipelaGO
{
    public class Bootstrap : MonoBehaviour
    {
        #region Fields
        #if UNITY_EDITOR
        [SerializeField]
        private bool m_skipSplashScreen = false;
        #endif

        [SerializeField]
        private float m_delayBeforeLoadingFirstScene = 5f;

        [SerializeField]
        private SplashScreenController m_splashScreen = null;

        [SerializeField]
        private Scene m_firstSceneToLoad = Scene.WorldMap;
        #endregion


        #region MonoBehaviour Implementation
        private void Start() => StartCoroutine(BootRoutine());
        #endregion


        #region Internal Methods
        private IEnumerator BootRoutine()
        {
            #if UNITY_EDITOR
            if (m_skipSplashScreen)
                goto LoadFirstScene;
            #endif

            if (m_splashScreen != null)
            {
                yield return m_splashScreen.PlaybackRoutine();
                yield return new WaitForSeconds(m_delayBeforeLoadingFirstScene);
            }

            LoadFirstScene:
            SceneLoader.LoadScene(m_firstSceneToLoad);
        }
        #endregion
    }
}