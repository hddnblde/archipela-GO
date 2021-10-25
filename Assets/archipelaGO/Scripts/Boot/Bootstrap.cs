using UnityEngine;
using System.Collections;
using archipelaGO.SceneHandling;

namespace archipelaGO
{
    public class Bootstrap : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private string m_playerName = "Player1";

        [Space]

        [SerializeField]
        private SplashScreenController m_splashScreen = null;

        [SerializeField]
        private float m_delayAfterSplashScreen = 5f;

        [Space]

        [SerializeField]
        private Scene m_firstSceneToLoad = Scene.WorldMap;
        #endregion


        #region MonoBehaviour Implementation
        private void Start() => StartCoroutine(BootRoutine());
        #endregion


        #region Internal Methods
        private IEnumerator BootRoutine()
        {
            GameData.GameDataHandler.Load(m_playerName);

            if (m_splashScreen != null)
            {
                yield return m_splashScreen.PlaybackRoutine();
                yield return new WaitForSeconds(m_delayAfterSplashScreen);
            }

            SceneLoader.LoadScene(m_firstSceneToLoad);
        }
        #endregion
    }
}