using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GridWord = archipelaGO.Puzzle.WordPuzzle.GridWord;

namespace archipelaGO.Puzzle
{
    public class WordHuntPuzzle : WordPuzzleController
    {
        private class WordHuntHint : WordHint
        {
            public WordHuntHint (int order, string word) =>
                (m_order, m_word) = (order, word);

            private int m_order;
            private string m_word;

            public override string GetHint() => $"{ m_order }. { m_word }";
        }

        private readonly string m_lettersInTheAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        protected override void OnPuzzleCompleted()
        {
            Debug.Log("Word Hunt Puzzle: Puzzle finished! You have won!!");
        }

        protected override void AssignCharacterToCell(char character, int characterIndex, int puzzleIndex, WordCell cell)
        {
            cell.SetAsCharacterTile(character);
            cell.SetState(WordCell.State.CharacterShown);
        }

        protected override void InitializeEmptyCell(WordCell cell)
        {
            cell.SetAsEmptyTile(false);
            cell.SetAsCharacterTile(GenerateRandomCharacter());
        }

        protected override WordHint GenerateHint(int order, GridWord gridWord) =>
            new WordHuntHint(order, gridWord.word.title);

        protected override string GenerateHintText(List<WordHint> hints)
        {
            StringBuilder wordHuntHints = new StringBuilder();

            foreach (WordHint hint in hints)
                wordHuntHints.AppendLine(hint.GetHint());

            return $"<b>Words</b>\n<i>{ wordHuntHints }</i>";
        }

        private char GenerateRandomCharacter()
        {
            int randomIndex = Random.Range(0, m_lettersInTheAlphabet.Length);

            return m_lettersInTheAlphabet[randomIndex];
        }
    }
}