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
        private string m_name;

        [SerializeField]
        private Sprite m_sprite;
        #endregion


        #region Properties
        public Sprite sprite => m_sprite;
        public string characterName => m_name;
        #endregion
    }
}