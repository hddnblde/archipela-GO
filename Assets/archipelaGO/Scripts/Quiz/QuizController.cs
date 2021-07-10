using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using archipelaGO.UI.Windows;
using WaitForChosenOption = archipelaGO.UI.Windows.ChoiceWindow.WaitForChosenOption;
using Question = archipelaGO.Quiz.QuizConfig.Question;

namespace archipelaGO.Quiz
{
    public class QuizController : MonoBehaviour
    {
        #region Fields
        [SerializeField, Range(5, 20)]
        private int m_items = 10;

        [SerializeField]
        private QuizConfig m_quiz = null;

        [SerializeField]
        private ChoiceWindow m_choiceWindow = null;

        private Coroutine m_quizRoutine = null;
        private int m_guessedCorrectAnswers = 0;
        #endregion


        #region MonoBehaviour Implementation
        private void Start() => BeginQuizRoutine();
        #endregion


        #region Internal Methods
        private void BeginQuizRoutine()
        {
            if (m_choiceWindow == null)
                return;

            if (m_quizRoutine != null)
                StopCoroutine(m_quizRoutine);

            List<Question> questions = GenerateQuestions();

            if (questions == null)
                return;

            m_quizRoutine = StartCoroutine(QuizRoutine(questions));
        }

        private List<Question> GenerateQuestions()
        {
            if (m_quiz == null || m_items <= 0)
                return null;

            return m_quiz.GenerateRandomSetOfQuestions(m_items);
        }

        private IEnumerator QuizRoutine(List<Question> questions)
        {
            m_guessedCorrectAnswers = 0;

            foreach (Question question in questions)
            {
                (string stem, string[] choices, int correctAnswerIndex) problem = question.GetProblem();

                WaitForChosenOption quizChoice =
                    m_choiceWindow.Show(problem.stem, problem.choices, false);

                yield return quizChoice;

                if (quizChoice.choiceIndex == problem.correctAnswerIndex)
                    m_guessedCorrectAnswers++;
            }

            Debug.Log($"Quiz finished! Guessed correct answers = { m_guessedCorrectAnswers }");
        }
        #endregion
    }
}