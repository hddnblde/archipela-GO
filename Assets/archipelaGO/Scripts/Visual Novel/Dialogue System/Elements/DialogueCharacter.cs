using UnityEngine;

namespace archipelaGO.VisualNovel.DialogueSystem.Elements
{
    [System.Serializable]
    public enum DialogueCharacterBlocking
    {
        StageLeft = 0,
        StageRight = 1
    }

    [System.Serializable]
    public class DialogueCharacter
    {
        #region Fields
        [SerializeField]
        private DialogueCharacterBlocking m_blocking;

        [SerializeField]
        private Sprite m_sprite;

        [SerializeField]
        private string m_name;
        #endregion


        #region Properties
        public DialogueCharacterBlocking blocking => m_blocking;
        public Sprite sprite => m_sprite;
        public string characterName => m_name;
        #endregion
    }
}