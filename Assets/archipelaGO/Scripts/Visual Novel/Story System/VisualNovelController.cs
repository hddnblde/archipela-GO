using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using archipelaGO.UI.Windows;
using DialogueCharacter = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueCharacter;
using Dialogue = archipelaGO.VisualNovel.DialogueSystem.Dialogue;
using DialogueLine = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueLine;
using WaitForChosenOption = archipelaGO.UI.Windows.ChoiceWindow.WaitForChosenOption;

namespace archipelaGO.VisualNovel.StorySystem
{
    public sealed class VisualNovelController : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Image m_backgroundImage = null;

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
        public IEnumerator ShowDialogue(DialogueCharacter character, Dialogue dialogue, WordBank wordBank)
        {
            for (int i = 0; i < dialogue.lineCount; i++)
            {
                DialogueLine line = dialogue.GetLine(i);
                PlayVoiceOverAudio(line.voiceOver);

                yield return m_dialogueWindow.
                    ShowDialogueLine(character.sprite,
                    character.characterName, line.text, wordBank);
            }
        }

        public void SetBackgroundScene(Sprite sprite)
        {
            if (m_backgroundImage != null)
                m_backgroundImage.sprite = sprite;
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