using UnityEngine;
using System.Collections;
using archipelaGO.SceneHandling;
using WaitForSignIn = archipelaGO.Boot.SignInManager.WaitForSignIn;

namespace archipelaGO.Boot
{
    public class Bootstrap : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private SignInManager m_signInManager = null;

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
            if (m_signInManager == null)
            {
                Debug.LogError("Failed to sign in because there was no manager assigned!");
                yield break;
            }

            WaitForSignIn signIn =
                m_signInManager.SignIn();

            yield return signIn;

            GameData.GameDataHandler.Load(signIn.saveSlot);

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