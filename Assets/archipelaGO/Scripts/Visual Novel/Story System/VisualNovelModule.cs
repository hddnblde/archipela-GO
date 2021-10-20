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
        private int m_currentPlot = -1;
        #endregion


        #region Game Module Implementation
        protected override void OnInitialize()
        {
            InitializeVisualNovelController();
            StopPlot();
            m_plotRoutine = StartCoroutine(PlotPlayback());
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

        private IEnumerator PlotPlayback()
        {
            yield return new WaitForSeconds(m_startDelay);

            m_currentPlot = 0;

            for (; m_currentPlot < config.plotlineCount; m_currentPlot++)
            {
                (Narrative narrative, Sprite scene, DialogueCharacter mainCharacter) plotline = config.GetPlotline(m_currentPlot);
                yield return SetScene(plotline.scene);
                yield return PlayNarrative(plotline.narrative, config.wordBank, plotline.mainCharacter);
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


        #if ARCHIPELAGO_DEBUG_MODE
        public override IEnumerator Debug_Autoplay()
        {
            Debug.LogWarning("Visual Novel Module does not support autoplay!");
            yield return null;
        }
        #endif
    }
}