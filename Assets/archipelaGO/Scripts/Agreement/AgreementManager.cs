using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace archipelaGO.UI.Windows
{
    public class AgreementManager : UIWindow
    {
        #region Fields
        #if UNITY_EDITOR
        [SerializeField]
        private bool m_resetChoices = false;
        #endif

        [SerializeField]
        private TextAsset m_EULA = null;

        [SerializeField]
        private TextAsset m_privacyPolicy = null;

        [SerializeField]
        private ScrollRect m_scrollRect = null;

        [SerializeField]
        private TextMeshProUGUI m_agreementText = null;

        [SerializeField]
        private Button m_acceptButton = null;

        [SerializeField]
        private Button m_declineButton = null;

        private AgreementDecision m_currentDecision = AgreementDecision.Undecided;
        private AgreementPolicy m_currentPolicy = AgreementPolicy.EULA;

        private const string AgreementKey_EULA = "Agreement.EULA",
            AgreementKey_PrivacyPolicy = "Agreement.PrivacyPolicy";
        #endregion


        #region Data Structure
        private enum AgreementPolicy
        {
            EULA = 0,
            Privacy = 1
        }

        private enum AgreementDecision
        {
            Declined = -1,
            Undecided = 0,
            Accepted = 1
        }
        #endregion


        #region Properties
        public static bool playerHasFullyAgreed => (agreedToEULA && agreedToPrivacyPolicy);

        private static bool agreedToEULA
        {
            get => PlayerPrefs.GetInt(AgreementKey_EULA, 0) == 1;
            set => PlayerPrefs.SetInt(AgreementKey_EULA, (value ? 1 : 0));
        }

        private static bool agreedToPrivacyPolicy
        {
            get => PlayerPrefs.GetInt(AgreementKey_PrivacyPolicy, 0) == 1;
            set => PlayerPrefs.SetInt(AgreementKey_PrivacyPolicy, (value ? 1 : 0));
        }
        #endregion


        #region MonoBehaviour Implementation
        protected override void Awake()
        {
            base.Awake();
            Hide();

            #if UNITY_EDITOR
            if (m_resetChoices)
            {
                agreedToEULA = false;
                agreedToPrivacyPolicy = false;
            }
            #endif

            RegisterResponseButtons();
        }
        
        private void Start()
        {
            if (playerHasFullyAgreed)
            {
                Destroy(gameObject);
                return;
            }

            Show();
            ActivateDecisionButtons(false);
            StartCoroutine(ShowAgreementRoutine());
        }

        private void OnDestroy() =>
            DeregisterResponseButtons();
        #endregion


        #region Internal Methods
        private void RegisterResponseButtons()
        {
            if (m_acceptButton != null)
                m_acceptButton.onClick.AddListener(OnAcceptButtonClicked);
            
            if (m_declineButton != null)
                m_declineButton.onClick.AddListener(OnDeclineButtonClicked);
        }

        private void DeregisterResponseButtons()
        {
            if (m_acceptButton != null)
                m_acceptButton.onClick.RemoveListener(OnAcceptButtonClicked);

            if (m_declineButton != null)
                m_declineButton.onClick.RemoveListener(OnDeclineButtonClicked);
        }

        private void MakeDecision()
        {
            m_currentDecision = AgreementDecision.Undecided;
            ActivateDecisionButtons(true);
        }

        private void ActivateDecisionButtons(bool active)
        {
            if (m_acceptButton != null)
                m_acceptButton.gameObject.SetActive(active);

            if (m_declineButton != null)
                m_declineButton.gameObject.SetActive(active);
        }

        private void OnAcceptButtonClicked()
        {
            m_currentDecision = AgreementDecision.Accepted;
            ActivateDecisionButtons(false);
        }

        private void OnDeclineButtonClicked()
        {
            m_currentDecision = AgreementDecision.Declined;
            ActivateDecisionButtons(false);
        }

        private IEnumerator ShowAgreementRoutine()
        {
            yield return ShowAgreementPolicy(AgreementPolicy.EULA);
            yield return ShowAgreementPolicy(AgreementPolicy.Privacy);

            if (playerHasFullyAgreed)
                Destroy(gameObject);

            else
            {
                Debug.LogWarning("Player has not fully agreed with the terms.");
                m_agreementText.text = "You must fully agree before using the app. Application will now quit.";

                yield return new WaitForSeconds(3f);
                Application.Quit();
            }
        }

        private IEnumerator ShowAgreementPolicy(AgreementPolicy policy)
        {
            m_currentPolicy = policy;

            if (m_agreementText != null)
                m_agreementText.text = RetrieveAgreementText(policy);

            MakeDecision();

            yield return new WaitUntil(()
                => (m_currentDecision != AgreementDecision.Undecided));

            SetPolicyDecision(policy, m_currentDecision);
        }

        private void SetPolicyDecision(AgreementPolicy policy, AgreementDecision decision)
        {
            bool accepted = (decision == AgreementDecision.Accepted);

            switch (policy)
            {
                case AgreementPolicy.EULA:
                    agreedToEULA = accepted;
                    break;

                case AgreementPolicy.Privacy:
                    agreedToPrivacyPolicy = accepted;
                    break;
            }
        }

        private string RetrieveAgreementText(AgreementPolicy policy)
        {
            switch (policy)
            {
                case AgreementPolicy.EULA:
                    return m_EULA?.text ?? string.Empty;

                case AgreementPolicy.Privacy:
                    return m_privacyPolicy?.text ?? string.Empty;

                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}