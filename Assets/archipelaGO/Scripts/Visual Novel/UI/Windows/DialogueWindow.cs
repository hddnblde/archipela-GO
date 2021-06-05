using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace archipelaGO.VisualNovel.UI.Windows
{
    public sealed class DialogueWindow : UIWindow
    {
        #region Fields
        [SerializeField]
        private AnimatedText m_animatedText = null;

        [SerializeField]
        private Button m_skipButton = null;

        [SerializeField]
        private bool m_showNextDialogue = false;
        #endregion


        #region MonoBehaviour Implementation
        private void OnEnable() => AddListenerToSkipButton();
        private void OnDisable() => RemoveListenerFromSkipButton();
        #endregion


        #region Skip Button Implementation
        private void AddListenerToSkipButton()
        {
            if (m_skipButton != null)
                m_skipButton.onClick.AddListener(OnSkipButtonPressed);
        }

        private void RemoveListenerFromSkipButton()
        {
            if (m_skipButton != null)
                m_skipButton.onClick.RemoveListener(OnSkipButtonPressed);
        }

        private void OnSkipButtonPressed()
        {
            if (m_animatedText != null)
                m_animatedText.SkipTextScrolling();
        }
        #endregion


        #region Public Methods
        public override void Hide()
        {
            m_animatedText.SkipTextScrolling();
            m_showNextDialogue = false;
            base.Hide();
        }

        public IEnumerator ShowDialogueLine(Sprite characterSprite, string characterName, string dialogueLine)
        {
            m_showNextDialogue = false;
            Show();
            SetCharacter(characterSprite, characterName);
            yield return AnimateDialogueLine(dialogueLine);
            yield return new WaitUntil(() => m_showNextDialogue);
            Hide();
        }
        #endregion


        #region Internal Methods
        private void SetCharacter(Sprite sprite, string name)
        {
            
        }

        private IEnumerator AnimateDialogueLine(string dialogueLine)
        {
            if (m_animatedText != null)
                yield return m_animatedText.ShowText(dialogueLine, Color.white);
        }
        #endregion
    }
}