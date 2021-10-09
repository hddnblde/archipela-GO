using UnityEngine;
using UnityEngine.UI;

namespace archipelaGO.Puzzle
{
    public abstract class WordCellRevealAnimation : ScriptableObject
    {
        public abstract void Animate(Text text, float t);
    }
}