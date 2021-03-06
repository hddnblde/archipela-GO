using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using archipelaGO.UI.Windows;
using DialogueCharacter = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueCharacter;
using Dialogue = archipelaGO.VisualNovel.DialogueSystem.Dialogue;
using DialogueLine = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueLine;
using WaitForChosenOption = archipelaGO.UI.Windows.ChoiceWindow.WaitForChosenOption;
using StageBlocking = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueCharacterBlocking;

namespace archipelaGO.VisualNovel.StorySystem
{
    public sealed class VisualNovelController : MonoBehaviour
    {
        #region Fields
        [Header ("Scenes")]

        [SerializeField]
        private Image m_backgroundImage = null;

        [Space]

        [SerializeField]
        private float m_sceneTransitionTime = 0.35f;

        [SerializeField]
        private Image m_screenBlocker = null;

        [Header ("Windows")]

        [SerializeField]
        private DialogueWindow m_dialogueWindow = null;

        [SerializeField]
        private ChoiceWindow m_choiceWindow = null;

        [Header("Audio")]

        [SerializeField]
        private AudioSource m_uiSFX = null;

        [SerializeField]
        private AudioSource m_bgm = null;

        [SerializeField]
        private AudioSource m_voiceOver = null;
        #endregion


        #region MonoBehaviour Implementation
        private void Awake() => HideAllWindows();
        #endregion


        #region Public Methods
        public IEnumerator ShowDialogue(DialogueCharacter character, StageBlocking blocking, Dialogue dialogue, WordBank wordBank, bool isMainCharacter)
        {
            if (character == null)
            {
                yield return new WaitForSeconds(0.7f);
                yield break;
            }

            string displayedCharacterName = character.displayedName;

            if (isMainCharacter)
                displayedCharacterName += " (You)";

            for (int i = 0; i < dialogue.lineCount; i++)
            {
                DialogueLine line = dialogue.GetLine(i);
                PlayVoiceOverAudio(line.voiceOver);

                yield return m_dialogueWindow.
                    ShowDialogueLine(character.sprite, character.pivotOffset,
                        character.scale, blocking, displayedCharacterName,
                        line.text, wordBank);
            }
        }

        public IEnumerator SetBackgroundScene(Sprite sprite)
        {
            if (m_backgroundImage == null)
                yield break;

            if (m_backgroundImage.sprite == sprite)
                yield break;

            yield return FadeScreen(false);
            m_backgroundImage.sprite = sprite;
            yield return FadeScreen(true);
        }

        private IEnumerator FadeScreen(bool fadeIn)
        {
            float start = (fadeIn ? 1f : 0f);
            float end = (fadeIn ? 0f : 1f);

            SetScreenBlockerToBlockRaycast(start > 0f);

            for (float current = 0f; current < m_sceneTransitionTime; current += Time.deltaTime)
            {
                float t = Mathf.InverseLerp(0f, m_sceneTransitionTime, current);
                float alpha = Mathf.Lerp(start, end, t);
                SetScreenBlockerAlpha(alpha);
                yield return null;
            }

            SetScreenBlockerAlpha(end);
            SetScreenBlockerToBlockRaycast(end < 0f);
        }

        private void SetScreenBlockerAlpha(float alpha)
        {
            if (m_screenBlocker == null)
                return;

            m_screenBlocker.color = new Color(m_screenBlocker.color.r,
                m_screenBlocker.color.g, m_screenBlocker.color.b, alpha);
        }

        private void SetScreenBlockerToBlockRaycast(bool block)
        {
            if (m_screenBlocker != null)
                m_screenBlocker.raycastTarget = block;
        }

        public WaitForChosenOption ShowAndChooseOption(string title, string[] choices) =>
            m_choiceWindow.Show(title, choices, true);

        public void HideAllWindows()
        {
            m_dialogueWindow.Hide();
            m_choiceWindow.Hide();
            StopVoiceOver();
        }
        #endregion


        #region Internal Methods
        private void PlayVoiceOverAudio(AudioClip voAudioClip)
        {
            if (m_voiceOver == null)
                return;

            StopVoiceOver();

            if (voAudioClip == null)
                return;

            m_voiceOver.clip = voAudioClip;
            m_voiceOver.Play();
        }

        private void StopVoiceOver()
        {
            if (m_voiceOver != null &&
                m_voiceOver.isPlaying)
                m_voiceOver.Stop();
        }
        #endregion
    }
}