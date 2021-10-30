using UnityEngine;
using System.Collections;
using archipelaGO.SceneHandling;
using WaitForSignIn = archipelaGO.Boot.SignInManager.WaitForSignIn;

namespace archipelaGO.Boot
{
    public class SignInScreen : TransitionableScreen
    {
        #region Fields
        [SerializeField]
        private SignInManager m_signInManager = null;
        #endregion


        #region Internal Methods
        public override IEnumerator WaitUntilDone()
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
            yield return null;
        }
        #endregion
    }
}