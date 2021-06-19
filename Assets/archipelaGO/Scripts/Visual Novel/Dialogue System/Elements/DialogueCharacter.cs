using UnityEngine;

namespace archipelaGO.VisualNovel.DialogueSystem.Elements
{
    [System.Serializable]
    public class DialogueCharacter
    {
        #region Fields
        [SerializeField]
        private Sprite m_sprite;

        [SerializeField]
        private string m_name;
        #endregion


        #region Properties
        public Sprite sprite => m_sprite;
        public string characterName => m_name;
        #endregion
    }
}