using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using StageBlocking = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueCharacterBlocking;

namespace archipelaGO.UI.Windows
{
    public sealed class DialogueWindow : UIWindow
    {
        #region Fields
        [SerializeField]
        private AnimatedText m_animatedText = null;

        [SerializeField]
        private Button m_skipButton = null;

        [Space]

        [SerializeField]
        private Image m_characterStageLeft = null;
        
        [SerializeField]
        private Image m_characterStageRight = null;

        [SerializeField]
        private Text m_characterNameText = null;

        private bool m_showNextDialogue = false;
        #endregion


        #region MonoBehaviour Implementation
        private void OnEnable() => AddEventListeners();
        private void OnDisable() => RemoveEventListeners();
        #endregion


        #region Skip Button Implementation
        private void AddEventListeners()
        {
            if (m_skipButton != null)
                m_skipButton.onClick.AddListener(OnSkipButtonPressed);
        }

        private void RemoveEventListeners()
        {
            if (m_skipButton != null)
                m_skipButton.onClick.RemoveListener(OnSkipButtonPressed);
        }

        private void OnSkipButtonPressed()
        {
            if (m_animatedText != null)
                m_animatedText.SkipTextScrolling();

            m_showNextDialogue = true;
        }
        #endregion


        #region Public Methods
        public override void Hide()
        {
            m_animatedText.SkipTextScrolling();
            m_showNextDialogue = false;
            base.Hide();
        }

        public IEnumerator ShowDialogueLine(Sprite characterSprite, StageBlocking stageBlocking,
            string characterName, string dialogueLine, WordBank wordBank)
        {
            m_showNextDialogue = false;
            Show();
            SetCharacter(characterSprite, stageBlocking, characterName);
            yield return AnimateDialogueLine(dialogueLine, wordBank);
            yield return new WaitUntil(() => m_showNextDialogue);
            Hide();
        }
        #endregion


        #region Internal Methods
        private void SetCharacter(Sprite sprite, StageBlocking blocking, string name)
        {
            if (m_characterNameText != null)
                m_characterNameText.text = name;

            AssignCharacterSprite(m_characterStageLeft,
                (blocking == StageBlocking.StageLeft ? sprite : null));

            AssignCharacterSprite(m_characterStageRight,
                (blocking == StageBlocking.StageRight ? sprite : null));
        }

        private void AssignCharacterSprite(Image characterImage, Sprite sprite)
        {
            if (characterImage == null)
                return;

            characterImage.sprite = sprite;
            characterImage.enabled = (sprite != null);
        }

        private IEnumerator AnimateDialogueLine(string dialogueLine, WordBank wordBank)
        {
            if (m_animatedText == null)
                yield break;

            string postProcessedDialogueLine =
                ApplyHyperlinkPostProcessingToDialogueLine(dialogueLine, wordBank);

            yield return m_animatedText.ShowText(postProcessedDialogueLine, Color.white, wordBank);
        }

        private string ApplyHyperlinkPostProcessingToDialogueLine(string line, WordBank wordBank)
        {
            Regex regex = new Regex(@"{[a-zA-Z\s]*}");
            MatchCollection matches = regex.Matches(line);
            List<(string, string)> postProcesors = new List<(string, string)>();

            for (int i = 0; i < matches.Count; i++)
            {
                string match = matches[i].Value;
                string normalizedMatch = match.Substring(1, match.Length - 2);
                int wordIndex = wordBank.GetWordIndex(normalizedMatch.ToLower());

                string postProcessedWord = (wordIndex != -1 ?
                    $"<style=Link><link=\"{ wordIndex }\">{ normalizedMatch }</link></style>" :
                    match);

                postProcesors.Add((match, postProcessedWord));
            }

            foreach ((string key, string value) postProcessor in postProcesors)
                line = line.Replace(postProcessor.key, postProcessor.value);

            return line;
        }
        #endregion
    }
}