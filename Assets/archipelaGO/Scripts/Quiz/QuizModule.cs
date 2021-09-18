using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameModule = archipelaGO.Game.GameModule<archipelaGO.Quiz.QuizConfig>;
using archipelaGO.UI.Windows;
using WaitForChosenOption = archipelaGO.UI.Windows.ChoiceWindow.WaitForChosenOption;

namespace archipelaGO.Quiz
{
    public class QuizModule : GameModule
    {
        #region Fields
        [SerializeField, Range(5, 20)]
        private int m_items = 10;

        [SerializeField]
        private ChoiceWindow m_choiceWindow = null;

        private QuizConfig m_quiz = null;
        private Coroutine m_quizRoutine = null;
        private int m_guessedCorrectAnswers = 0;
        #endregion


        #region Game Module Implementation
        public override void Initialize(QuizConfig config)
        {
            m_quiz = config;
            BeginQuizRoutine();
        }
        #endregion


        #region Internal Methods
        private void BeginQuizRoutine()
        {
            if (m_choiceWindow == null)
                return;

            if (m_quizRoutine != null)
                StopCoroutine(m_quizRoutine);

            List<(string, string[], int[])> questions = GenerateQuestions();

            if (questions == null)
                return;

            m_quizRoutine = StartCoroutine(QuizRoutine(questions));
        }

        private List<(string, string[], int[])> GenerateQuestions()
        {
            if (m_quiz == null || m_items <= 0)
                return null;

            return m_quiz.GenerateRandomSetOfQuestions(m_items);
        }

        private IEnumerator QuizRoutine(List<(string, string[], int[])> questions)
        {
            m_guessedCorrectAnswers = 0;

            foreach ((string stem, string[] choices, int[] correctAnswers) question in questions)
            {
                // (string stem, string[] choices, int correctAnswerIndex) problem = question.GetProblem();

                WaitForChosenOption quizChoice =
                    m_choiceWindow.Show(question.stem, question.choices, false);

                yield return quizChoice;

                if (question.correctAnswers.Contains(quizChoice.choiceIndex))
                    m_guessedCorrectAnswers++;
            }

            //TODO: clear screen
            Debug.Log($"Quiz finished! Guessed correct answers = { m_guessedCorrectAnswers }");
            InvokeGameCompleted();
        }
        #endregion
    }
}