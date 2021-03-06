using System.Collections.Generic;
using UnityEngine;

namespace archipelaGO.VisualNovel.DialogueSystem.Elements
{
    [CreateAssetMenu(fileName = "Character Set", menuName = "archipelaGO/Visual Novel/Character Set")]
    public class CharacterSet : ScriptableObject
    {
        [SerializeField]
        List<DialogueCharacter> m_characters = new List<DialogueCharacter>();

        public int Count => m_characters.Count;

        public DialogueCharacter GetCharacter(int index)
        {
            if (index < 0 || index >= Count)
                return null;

            return m_characters[index];
        }
    }
}