using System.Collections;
using UnityEngine;
using GameModule = archipelaGO.Game.GameModule<archipelaGO.VisualNovel.StorySystem.Plot>;
using Conversation = archipelaGO.VisualNovel.DialogueSystem.Conversation;
using Dialogue = archipelaGO.VisualNovel.DialogueSystem.Dialogue;
using DialogueCharacter = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueCharacter;
using WaitForChosenOption = archipelaGO.UI.Windows.ChoiceWindow.WaitForChosenOption;
using StageBlocking = archipelaGO.VisualNovel.DialogueSystem.Elements.DialogueCharacterBlocking;

namespace archipelaGO.VisualNovel.StorySystem
{
    [RequireComponent(typeof(VisualNovelController))]
    public class VisualNovelModule : GameModule
    {
        #region Fields
        [SerializeField]
        private float m_startDelay = 1.85f;

        private VisualNovelController m_vnController = null;
        private Coroutine m_plotRoutine = null;
        #endregion


        #region Game Module Implementation
        public override void Initialize(Plot config)
        {
            InitializeVisualNovelController();
            StopPlot();
            m_plotRoutine = StartCoroutine(PlotPlayback(config));
        }
        #endregion


        #region Internal Methods
        private void StopPlot()
        {
            if (m_plotRoutine != null)
                StopCoroutine(m_plotRoutine);

            m_vnController.HideAllWindows();
        }

        private void InitializeVisualNovelController() =>
            m_vnController = GetComponent<VisualNovelController>();

        private IEnumerator SetScene(Sprite scene) =>
            m_vnController.SetBackgroundScene(scene);

        private IEnumerator PlotPlayback(Plot plot)
        {
            yield return new WaitForSeconds(m_startDelay);

            for (int i = 0; i < plot.plotlineCount; i++)
            {
                (Narrative narrative, Sprite scene, DialogueCharacter mainCharacter) plotline = plot.GetPlotline(i);
                yield return SetScene(plotline.scene);
                yield return PlayNarrative(plotline.narrative, plot.wordBank, plotline.mainCharacter);
            }

            InvokeGameCompleted();
        }

        private IEnumerator PlayNarrative(Narrative narrative, WordBank wordBank, DialogueCharacter mainCharacter)
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
                (DialogueCharacter character, StageBlocking blocking, Dialogue dialogue) dialogueLine =
                    conversation.GetDialogueLine(i);

                bool isMainCharacter = (mainCharacter != null && dialogueLine.character == mainCharacter);

                yield return m_vnController.
                    ShowDialogue(dialogueLine.character, dialogueLine.blocking,
                        dialogueLine.dialogue, wordBank, isMainCharacter);
            }
        }
        #endregion
    }
}