using UnityEngine;

namespace archipelaGO.GameData
{
    public class AgreementData : BaseGameData
    {
        [SerializeField]
        private bool m_agreedToEula = false;

        [SerializeField]
        private bool m_agreedToPrivacyPolicy = false;

        public bool agreedToEula => m_agreedToEula;
        public bool agreedToPrivacyPolicy => m_agreedToPrivacyPolicy;

        public override void Reset()
        {
            m_agreedToEula = false;
            m_agreedToPrivacyPolicy = false;
        }

        public void AgreeToEndUserLicenseAgreement(bool agree) =>
            m_agreedToEula = agree;
        
        public void AgreeToPrivacyPolicy(bool agree) =>
            m_agreedToPrivacyPolicy = agree;
    }
}