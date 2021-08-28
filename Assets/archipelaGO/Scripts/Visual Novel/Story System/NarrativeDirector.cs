using System.Collections;
using UnityEngine;
using Conversation = archipelaGO.VisualNovel.DialogueSystem.Conversation;
using Dialogue = archipelaGO.VisualNovel.DialogueSystem.Dialogue;
using DialogueCharacter = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueCharacter;
using WaitForChosenOption = archipelaGO.UI.Windows.ChoiceWindow.WaitForChosenOption;

namespace archipelaGO.VisualNovel.StorySystem
{
    [RequireComponent(typeof(VisualNovelController))]
    public class NarrativeDirector : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float m_startDelay = 1.85f;

        private VisualNovelController m_vnController = null;
        private Coroutine m_plotRoutine = null;
        #endregion


        #region MonoBehaviour Implementation
        private void Awake() => InitializeVisualNovelController();
        #endregion


        #region Public Method
        public void PlayPlot(Plot plot)
        {
            if (plot == null)
                return;

            StopPlot();
            m_plotRoutine = StartCoroutine(PlotPlayback(plot));
        }

        private void StopPlot()
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
            yield return new WaitForSeconds(m_startDelay);

            for (int i = 0; i < plot.plotlineCount; i++)
            {
                (Narrative narrative, Sprite scene) plotline = plot.GetPlotline(i);
                SetScene(plotline.scene);
                yield return PlayNarrative(plotline.narrative, plot.wordBank);
            }
        }

        private IEnumerator PlayNarrative(Narrative narrative, WordBank wordBank)
        {
            string title;
            string[] choices;
            int choice = Narrative.FirstOption;

            if (narrative.GetChoices(out title, out choices))
            {
                WaitForChosenOption vnChoice = m_vnController.ShowAndChooseOption(title, choices);
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
                    ShowDialogue(dialogueLine.character, dialogueLine.dialogue, wordBank);
            }
        }
        #endregion
    }
}