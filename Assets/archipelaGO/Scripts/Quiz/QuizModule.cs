using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ScorableGameModule = archipelaGO.Game.ScorableGameModule<archipelaGO.Quiz.QuizConfig>;
using archipelaGO.UI.Windows;
using WaitForChosenOption = archipelaGO.UI.Windows.ChoiceWindow.WaitForChosenOption;

namespace archipelaGO.Quiz
{
    public class QuizModule : ScorableGameModule
    {
        #region Fields
        [SerializeField, Range(5, 20)]
        private int m_items = 10;

        [SerializeField]
        private float m_delayBeforeMovingToNextItem = 3.15f;

        [SerializeField]
        private ChoiceWindow m_choiceWindow = null;

        private Coroutine m_quizRoutine = null;
        private int m_guessedCorrectAnswers = 0;
        private int m_currentQuestionIndex = -1;
        private List<Question> m_questions = new List<Question>();
        #endregion


        #region Data Structure
        private class Question
        {
            public Question (string stem, string[] choices, int[] correctAnswers)
            {
                m_stem = stem;
                m_choices = choices;
                m_correctAnswers = correctAnswers;
            }

            private string m_stem = string.Empty;
            private string[] m_choices = null;
            private int[] m_correctAnswers = null;

            public string stem => m_stem;
            public string[] choices => m_choices;
            public int[] correctAnswerIndices => m_correctAnswers;
        }
        #endregion


        #region Scorable Module Implementation
        public override int currentScore => m_guessedCorrectAnswers;
        public override int totalScore => m_questions?.Count ?? 0;
        #endregion


        #region Game Module Implementation
        protected override void OnInitialize() =>
            BeginQuizRoutine();
        #endregion


        #region Internal Methods
        private void BeginQuizRoutine()
        {
            if (m_choiceWindow == null)
                return;

            if (m_quizRoutine != null)
                StopCoroutine(m_quizRoutine);

            m_questions = GenerateQuestions();

            if (m_questions == null)
                return;

            m_quizRoutine = StartCoroutine(QuizRoutine());
        }

        private List<Question> GenerateQuestions()
        {
            List<Question> questions = new List<Question>();

            if (config == null || m_items <= 0)
                return questions;

            List<(string, string[], int[])> setOfQuestions = config.GenerateRandomSetOfQuestions(m_items);

            if (setOfQuestions == null)
                return questions;
            
            foreach ((string stem, string[] choices, int[] correctAnswers) item in setOfQuestions)
            {
                Question question = new Question(item.stem, item.choices, item.correctAnswers);
                questions.Add(question);
            }

            return questions;
        }

        private IEnumerator QuizRoutine()
        {
            m_guessedCorrectAnswers = 0;
            m_currentQuestionIndex = 0;

            for (; m_currentQuestionIndex < m_questions.Count; m_currentQuestionIndex++)
            {
                Question question = GetCurrentQuestion();

                WaitForChosenOption quizChoice =
                    m_choiceWindow.Show(question.stem, question.choices, false);

                yield return quizChoice;

                if (question.correctAnswerIndices.Contains(quizChoice.choiceIndex))
                    m_guessedCorrectAnswers++;

                m_choiceWindow.ShowCorrectAnswer(quizChoice.choiceIndex, question.correctAnswerIndices);

                yield return new WaitForSeconds(m_delayBeforeMovingToNextItem);
            }

            //TODO: clear screen
            //TODO: medal screen
            Debug.Log($"Quiz finished! Guessed correct answers = { m_guessedCorrectAnswers }");
            // display medal first before ending
            InvokeGameOver();
        }

        private Question GetCurrentQuestion()
        {
            if (m_questions == null || m_currentQuestionIndex < 0 ||
                m_currentQuestionIndex >= m_questions.Count)
                return null;

            return m_questions[m_currentQuestionIndex];
        }
        #endregion


        #if ARCHIPELAGO_DEBUG_MODE
        public override IEnumerator Debug_Autoplay()
        {
            if (m_questions == null || m_questions.Count <= 0)
                yield break;

            int activeQuestion = m_currentQuestionIndex;

            while (m_currentQuestionIndex < m_questions.Count)
            {
                Question question = GetCurrentQuestion();

                if (question.correctAnswerIndices == null || question.correctAnswerIndices.Length <= 0)
                    yield break;

                m_choiceWindow.Debug_ChooseAnswer(question.correctAnswerIndices[0]);
                yield return new WaitUntil(() => activeQuestion != m_currentQuestionIndex);
                activeQuestion = m_currentQuestionIndex;
            }
        }
        #endif
    }
}