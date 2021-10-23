using UnityEngine;
using System.Collections;
using archipelaGO.SceneHandling;

namespace archipelaGO
{
    public class Bootstrap : MonoBehaviour
    {
        #region Fields
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
            if (m_splashScreen != null)
            {
                yield return m_splashScreen.PlaybackRoutine();
                yield return new WaitForSeconds(m_delayBeforeLoadingFirstScene);
            }

            SceneLoader.LoadScene(m_firstSceneToLoad);
        }
        #endregion
    }
}