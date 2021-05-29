using UnityEngine;
using UnityEngine.UI;
using archipelaGO.VisualNovel.UI.Windows;

namespace archipelaGO.VisualNovel.UI
{
    public sealed class VisualNovelController : MonoBehaviour
    {
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

        public void ShowDialogue()
        {

        }
    }
}