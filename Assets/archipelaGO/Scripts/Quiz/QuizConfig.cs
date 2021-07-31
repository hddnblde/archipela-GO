using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.Quiz
{
    [CreateAssetMenu(fileName = "Quiz", menuName = "archipelaGO/Quiz", order = 2)]
    public class QuizConfig : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private List<Question> m_questions = new List<Question>();
        #endregion


        #region Methods
        public List<Question> GenerateRandomSetOfQuestions(int count)
        {
            List<Question> shuffledQuestions = new List<Question>();
            shuffledQuestions.AddRange(m_questions);
            shuffledQuestions = ShuffleList<Question>(shuffledQuestions);

            if (shuffledQuestions.Count > count)
            {
                int difference = (shuffledQuestions.Count - count);
                shuffledQuestions.RemoveRange(count, difference);
            }

            return shuffledQuestions;
        }

        private static List<T> ShuffleList<T>(List<T> list)
        {
            List<T> shuffledList = new List<T>();
            shuffledList.AddRange(list);

            for (int i = 0; i < shuffledList.Count; i++)
            {
                T targetElement = shuffledList[i];
                int randomIndex = Random.Range(0, shuffledList.Count);
                T swappedElement = shuffledList[randomIndex];

                shuffledList[i] = swappedElement;
                shuffledList[randomIndex] = targetElement;
            }

            return shuffledList;
        }
        #endregion


        #region Data Structure
        [System.Serializable]
        public struct Question
        {
            #region Fields
            [SerializeField, TextArea(4, 12)]
            private string m_stem;

            [SerializeField, TextArea(3, 5)]
            private string m_correctAnswer;

            [SerializeField, TextArea(3, 5)]
            private List<string> m_incorrectAnswers;
            #endregion

            public (string stem, string[] choices, int correctAnswerIndex) GetProblem()
            {
                List<string> choices = new List<string>();
                choices.Add(m_correctAnswer);
                choices.AddRange(m_incorrectAnswers);
                choices = ShuffleList<string>(choices);

                int correctAnswerIndex = choices.IndexOf(m_correctAnswer);

                return (m_stem, choices.ToArray(), correctAnswerIndex);
            }
        }
        #endregion
    }
}