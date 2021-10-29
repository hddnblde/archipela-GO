using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using archipelaGO.UI;
using archipelaGO.GameData;

namespace archipelaGO.Boot
{
    public class SignInManager : UIWindow
    {
        #region Fields
        [SerializeField]
        private GameObject m_playerDataUIPrefab = null;

        [SerializeField]
        private RectTransform m_playersDataContainer = null;

        [SerializeField]
        private Text m_inputField = null;

        private Coroutine m_signInRoutine = null;
        #endregion

        #region Data Structure
        private delegate void SuccessfullySignedIn(string playerName);

        public class WaitForSignIn : CustomYieldInstruction
        {
            public WaitForSignIn(SignInManager signInManager)
            {
                if (signInManager != null)
                    signInManager.Show(OnPlayerSuccessfullySignedIn);
            }

            private string m_playerName = string.Empty;
            private bool m_successfullySignedIn = false;

            private void OnPlayerSuccessfullySignedIn(string playerName)
            {
                m_playerName = playerName;
                m_successfullySignedIn = true;
            }

            public string playerName => m_playerName;
            public override bool keepWaiting => m_successfullySignedIn;
        }
        #endregion
        

        #region Public Method
        public WaitForSignIn SignIn() => new WaitForSignIn(this);
        #endregion


        #region Internal Methods
        private void Show(SuccessfullySignedIn callback)
        {
            if (m_signInRoutine != null)
                StopCoroutine(m_signInRoutine);

            m_signInRoutine =
                StartCoroutine(SignInRoutine(callback));
        }

        private void LoadExistingPlayersData()
        {
            
        }

        private IEnumerator SignInRoutine(SuccessfullySignedIn signIn)
        {
            LoadExistingPlayersData();
            this.Show();

            yield return null;
        }
        #endregion
    }
}