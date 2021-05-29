using UnityEngine;
using archipelaGO.VisualNovel.DialogueSystem;

namespace archipelaGO.VisualNovel.StorySystem
{
    public abstract class Narrative : ScriptableObject
    {
        public abstract bool GetChoices(out string title, out string[] choices);
        public abstract Conversation GetConversation(int choice);
        public Conversation GetConversation() => GetConversation(-1);
    }
}