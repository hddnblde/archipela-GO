using System.Collections;
using UnityEngine;
using archipelaGO.VisualNovel.UI;
using Conversation = archipelaGO.VisualNovel.DialogueSystem.Conversation;
using Dialogue = archipelaGO.VisualNovel.DialogueSystem.Dialogue;
using DialogueCharacter = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueCharacter;
using VisualNovelChoice = archipelaGO.VisualNovel.UI.Windows.ChoiceWindow.VisualNovelChoice;

namespace archipelaGO.VisualNovel.StorySystem
{
    [RequireComponent(typeof(VisualNovelController))]
    public class NarrativeDirector : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Plot m_plot = null;

        private VisualNovelController m_vnController = null;
        private Coroutine m_plotRoutine = null;
        #endregion


        #region MonoBehaviour Implementation
        private void Awake() => InitializeVisualNovelController();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                PlayPlot();
        }
        #endregion


        #region Public Method
        public void PlayPlot()
        {
            if (m_plot == null)
                return;

            StopPlot();
            m_plotRoutine = StartCoroutine(PlotPlayback(m_plot));
        }

        public void StopPlot()
        {
            if (m_plotRoutine != null)
                StopCoroutine(m_plotRoutine);

            m_vnController.HideAllWindows();
        }
        #endregion


        #region Internal Methods
        private void InitializeVisualNovelController() =>
            m_vnController = GetComponent<VisualNovelController>();

        private void SetScene(Sprite scene) =>
            m_vnController.SetBackgroundScene(scene);

        private IEnumerator PlotPlayback(Plot plot)
        {
            for (int i = 0; i < plot.plotlineCount; i++)
            {
                (Narrative narrative, Sprite scene) plotline = plot.GetPlotline(i);
                SetScene(plotline.scene);
                yield return PlayNarrative(plotline.narrative);
            }
        }

        private IEnumerator PlayNarrative(Narrative narrative)
        {
            string title;
            string[] choices;
            int choice = Narrative.FirstOption;

            if (narrative.GetChoices(out title, out choices))
            {
                VisualNovelChoice vnChoice = m_vnController.ShowAndChooseOption(title, choices);
                yield return vnChoice;
                choice = vnChoice.choiceIndex;
            }

            Conversation conversation = narrative.GetConversation(choice);

            if (conversation == null)
                yield break;

            for (int i = 0; i < conversation.dialogueCount; i++)
            {
                (DialogueCharacter character, Dialogue dialogue) dialogueLine =
                    conversation.GetDialogueLine(i);

                yield return m_vnController.
                    ShowDialogue(dialogueLine.character, dialogueLine.dialogue);
            }
        }
        #endregion
    }
}