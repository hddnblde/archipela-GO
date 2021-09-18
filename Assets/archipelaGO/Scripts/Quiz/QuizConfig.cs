using System.Collections.Generic;
using UnityEngine;
using GameModuleConfig = archipelaGO.Game.GameModuleConfig;

namespace archipelaGO.Quiz
{
    [CreateAssetMenu(fileName = "Quiz", menuName = "archipelaGO/Game Module/Quiz", order = 2)]
    public class QuizConfig : GameModuleConfig
    {
        #region Fields
        [SerializeField]
        private bool m_shuffleQuestions = false;

        [SerializeField]
        private bool m_shuffleChoices = false;

        [SerializeField]
        private List<Question> m_questions = new List<Question>();
        #endregion


        #region Methods
        public List<(string stem, string[] choices, int[] correctAnswers)> GenerateRandomSetOfQuestions(int count)
        {
            List<Question> questions = new List<Question>();
            questions.AddRange(m_questions);

            if (m_shuffleQuestions)
                questions = ShuffleList<Question>(questions);

            if (questions.Count > count)
            {
                int difference = (questions.Count - count);
                questions.RemoveRange(count, difference);
            }

            return GenerateItems(questions, m_shuffleChoices);
        }
        
        private List<(string, string[], int[])> GenerateItems(List<Question> questions, bool shuffleChoices)
        {
            if (questions == null || questions.Count <= 0)
                return null;

            List<(string, string[], int[])> items = new List<(string, string[], int[])>();

            foreach (Question question in questions)
                items.Add(question.GetProblem(shuffleChoices));

            return items;
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
        private struct Question
        {
            #region Fields
            [SerializeField, TextArea(4, 12)]
            private string m_stem;

            [SerializeField]
            private List<Answer> m_answers;
            #endregion

            public (string stem, string[] choices, int[] answers) GetProblem(bool shuffleChoices)
            {
                (string[] choices, int[] correctAnswers) item =
                    GenerateItem(shuffleChoices);

                return (m_stem, item.choices, item.correctAnswers);
            }

            private (string[] choices, int[] correctAnswers) GenerateItem(bool shuffled)
            {
                List<Answer> answers = new List<Answer>();
                answers.AddRange(m_answers);

                if (shuffled)
                    answers = ShuffleList<Answer>(answers);
                
                List<string> choices = new List<string>();
                List<int> correctAnswers = new List<int>();

                for (int i = 0; i < answers.Count; i++)
                {
                    Answer answer = answers[i];
                    choices.Add(answer.text);

                    if (answer.isCorrect)
                        correctAnswers.Add(i);
                }

                return (choices.ToArray(), correctAnswers.ToArray());
            }
        }

        [System.Serializable]
        private struct Answer
        {
            [SerializeField, TextArea(2, 2)]
            private string m_text;

            [SerializeField]
            private bool m_isCorrect;


            public bool isCorrect => m_isCorrect;
            public string text => m_text;
        }
        #endregion
    }
}