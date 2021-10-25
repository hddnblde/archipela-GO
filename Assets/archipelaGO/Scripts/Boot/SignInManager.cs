using UnityEngine;

namespace archipelaGO.Boot
{
    public class SignInManager : MonoBehaviour
    {
        [SerializeField]
        private string m_signedInPlayer = string.Empty;

        public string signedInPlayer => m_signedInPlayer;
    }
}